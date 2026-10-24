using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Lab1
{
    public partial class MainPage : ContentPage
    {
        GoogleDriveHelper driveHelper = new GoogleDriveHelper();
        int CountCol = 10;
        int CountRow = 10;

        Spreadsheet spreadsheet = new Spreadsheet();
        Dictionary<string, Entry> uiCells = new Dictionary<string, Entry>();

     
        private bool showExpressions = false;

        public MainPage()
        {
            InitializeComponent();
            BuildGrid();
        }

        private void BuildGrid()
        {
            SpreadsheetGrid.Children.Clear();
            SpreadsheetGrid.RowDefinitions.Clear();
            SpreadsheetGrid.ColumnDefinitions.Clear();
            uiCells.Clear();

            for (int col = 0; col <= CountCol; col++)
            {
                SpreadsheetGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                if (col > 0)
                {
                    var label = new Label
                    {
                        Text = ((char)('A' + col - 1)).ToString(),
                        HorizontalOptions = LayoutOptions.Center,
                        FontAttributes = FontAttributes.Bold
                    };
                    SpreadsheetGrid.Add(label, col, 0);
                }
            }

            for (int row = 1; row <= CountRow; row++)
            {
                SpreadsheetGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var label = new Label
                {
                    Text = row.ToString(),
                    VerticalOptions = LayoutOptions.Center,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 0, 10, 0)
                };
                SpreadsheetGrid.Add(label, 0, row);

                for (int col = 1; col <= CountCol; col++)
                {
                    string cellName = $"{(char)('A' + col - 1)}{row}";

                    var entry = new Entry
                    {
                        WidthRequest = 100,
                        Text = "",
                        BackgroundColor = Colors.White
                    };

                    entry.Focused += (s, e) =>
                    {
                        CurrentCellLabel.Text = cellName;
                        FormulaEntry.Text = spreadsheet.Cells.ContainsKey(cellName) ? spreadsheet.Cells[cellName].Expression : "";
                        entry.BackgroundColor = Color.FromArgb("#A6B9BE"); 
                    };

                    entry.Completed += async (s, e) =>
                    {
                        try
                        {
                            spreadsheet.SetCell(cellName, entry.Text);
                            RefreshGrid();
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlertAsync("Помилка синтаксису", ex.Message, "ОК");                        
                            spreadsheet.Cells.Remove(cellName);
                            entry.Text = "";
                            RefreshGrid();
                        }
                    };

                    entry.Unfocused += async (s, e) =>
                    {
                        try
                        {
                            spreadsheet.SetCell(cellName, entry.Text);
                            RefreshGrid();
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlertAsync("Помилка синтаксису", ex.Message, "ОК");
                            spreadsheet.Cells.Remove(cellName);
                            entry.Text = "";
                            RefreshGrid();
                        }
                        entry.BackgroundColor = Colors.White;
                    };

                    uiCells[cellName] = entry;
                    SpreadsheetGrid.Add(entry, col, row);
                }
            }
        }

        private void OnModeToggled(object sender, EventArgs e)
        {
            showExpressions = !showExpressions;
            ModeButton.Text = showExpressions ? "Режим: Формули" : "Режим: Значення";
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            foreach (var cell in spreadsheet.Cells.Values)
            {
                if (uiCells.ContainsKey(cell.Name))
                {
                    uiCells[cell.Name].Text = showExpressions ? cell.Expression : cell.Value?.ToString();
                }
            }
        }

        private async void OnCalculateClicked(object sender, EventArgs e)
        {
            string currentCell = CurrentCellLabel.Text;
            string formula = FormulaEntry.Text;

            try
            {
                spreadsheet.SetCell(currentCell, formula);
                RefreshGrid();
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Помилка синтаксису", ex.Message, "ОК");
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                string json = JsonSerializer.Serialize(spreadsheet.Cells);
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "table.json");
                File.WriteAllText(path, json);

                await driveHelper.UploadFileAsync(path);

                await DisplayAlertAsync("Успіх", "Таблицю успішно збережено на Google Drive!", "ОК");
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Помилка", $"Не вдалося зберегти на Диск: {ex.Message}", "ОК");
            }
        }

        private async void OnLoadClicked(object sender, EventArgs e)
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "table.json");

                bool downloaded = await driveHelper.DownloadFileAsync(path);

                if (downloaded && File.Exists(path))
                { 
                    string json = File.ReadAllText(path);
                    spreadsheet.Cells = JsonSerializer.Deserialize<Dictionary<string, Cell>>(json);
                    RefreshGrid();
                    await DisplayAlertAsync("Успіх", "Таблицю завантажено з Google Drive!", "ОК");
                }
                else
                {
                    await DisplayAlertAsync("Увага", "Файл збереження не знайдено на вашому Google Drive.", "ОК");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Помилка", $"Не вдалося завантажити з Диска: {ex.Message}", "ОК");
            }
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            await DisplayAlertAsync("Допомога",
                "Лабораторна робота №1\n" +
                "Студент: Тракалюк Анжеліка\n\n" +
                "Інструкція:\n" +
                "1. Клікніть на клітинку.\n" +
                "2. Введіть вираз (наприклад A1+10) і натисніть Enter або 'Застосувати'.\n" +
                "3. Кнопка 'Режим' перемикає показ формул та результатів.",
                "Зрозуміло");
        }

        private async void OnResizeClicked(object sender, EventArgs e)
        {
            if (int.TryParse(ColCountEntry.Text, out int newCol) && int.TryParse(RowCountEntry.Text, out int newRow))
            {
                if (newCol > 0 && newCol <= 26 && newRow > 0)
                {
                    CountCol = newCol;
                    CountRow = newRow;

                    BuildGrid();
                    RefreshGrid();
                }
                else
                {
                    await DisplayAlertAsync("Помилка", "Кількість стовпців має бути від 1 до 26, а рядків - більше 0.", "ОК");
                }
            }
            else
            {
                await DisplayAlertAsync("Помилка", "Введіть цілі числа.", "ОК");
            }
        }
        private void OnClearCellClicked(object sender, EventArgs e)
        {
            string currentCell = CurrentCellLabel.Text;


            if (spreadsheet.Cells.ContainsKey(currentCell))
            {
                spreadsheet.Cells.Remove(currentCell);
            }

            FormulaEntry.Text = "";
            if (uiCells.ContainsKey(currentCell))
            {
                uiCells[currentCell].Text = "";
            }
        }

        private async void OnClearTableClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlertAsync("Підтвердження", "Ви впевнені, що хочете повністю очистити всю таблицю?", "Так", "Ні");
            if (confirm)
            {
                spreadsheet.Cells.Clear();
                FormulaEntry.Text = "";
                BuildGrid(); 
            }
        }

    }
}
