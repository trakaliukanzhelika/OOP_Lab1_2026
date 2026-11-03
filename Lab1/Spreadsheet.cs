namespace Lab1
{
    public class Spreadsheet
    {
        public Dictionary<string, Cell> Cells { get; set; } = new Dictionary<string, Cell>();

        public void SetCell(string name, string expression)
        {
            // Empty expression = the cell was cleared
            if (String.IsNullOrWhiteSpace(expression))
            {
                Cells.Remove(name);
                EvaluateAll();
                return;
            }

            if (!Cells.ContainsKey(name))
            {
                Cells[name] = new Cell(name);
            }
            Cells[name].Expression = expression;
            EvaluateAll();
        }

        public void EvaluateAll()
        {
            Dictionary<string, object> variables = new();
            foreach (Cell cell in Cells.Values)
            {
                variables[cell.Name] = cell.Value ?? 0;
            }

            // Errors of all cells are collected here, so one wrong cell
            // does not stop the recalculation of the others
            string errors = "";

            // Cells are calculated in the order they were added. If a cell refers
            // to a cell further in the list, it gets the new value only on the next pass.
            // So the passes are repeated until the values stop changing.
            for (int pass = 0; pass < Cells.Count; pass++)
            {
                errors = "";
                bool changed = false;

                foreach (Cell cell in Cells.Values)
                {
                    if (!String.IsNullOrWhiteSpace(cell.Expression))
                    {
                        object oldValue = cell.Value;
                        try
                        {
                            cell.Value = Calculator.Evaluate(cell.Expression, variables);
                            variables[cell.Name] = cell.Value;
                        }
                        catch (Exception ex)
                        {
                            cell.Value = "#ПОМИЛКА";
                            variables.Remove(cell.Name);
                            errors += $"{cell.Name}: {ex.Message}\n";
                        }

                        if (!Equals(oldValue, cell.Value))
                        {
                            changed = true;
                        }
                    }
                }

                if (!changed)
                {
                    break;
                }
            }

            if (errors != "")
            {
                throw new Exception(errors);
            }
        }

        // Removes cells that are outside the table after it was made smaller
        public void RemoveCellsOutside(int rowCount, int colCount)
        {
            List<string> toRemove = new List<string>();
            foreach (string name in Cells.Keys)
            {
                int col = name[0] - 'A' + 1;
                int row = int.Parse(name.Substring(1));
                if (col > colCount || row > rowCount)
                {
                    toRemove.Add(name);
                }
            }
            foreach (string name in toRemove)
            {
                Cells.Remove(name);
            }
        }
    }
}
