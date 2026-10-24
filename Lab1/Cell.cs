using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public class Cell
    {
        public string Name { get; set; }
        public string Expression { get; set; }
        public object Value { get; set; }

        public Cell(string name)
        {
            Name = name;
            Expression = "";
            Value = "";
        }
    }
}
