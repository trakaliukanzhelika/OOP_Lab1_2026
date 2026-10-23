using System;
using System.Collections.Generic;

namespace Lab1
{
    public class Spreadsheet
    {
        public Dictionary<string, Cell> Cells { get; set; } = new Dictionary<string, Cell>();

        public void SetCell(string name, string expression)
        {
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
            foreach (Cell cell in Cells.Values)
            {
                if (!String.IsNullOrWhiteSpace(cell.Expression))
                {
                    try
                    {
                        cell.Value = Calculator.Evaluate(cell.Expression, variables);
                        variables[cell.Name] = cell.Value;
                    }
                    catch (Exception ex)
                    {
                        cell.Value = ex.Message;
                    }
                }
            }
        }
    }
}
