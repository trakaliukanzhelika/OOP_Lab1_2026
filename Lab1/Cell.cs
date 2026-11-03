using System.Text.Json.Serialization;

namespace Lab1
{
    public class Cell
    {
        public string Name { get; set; }
        public string Expression { get; set; }

        // Not saved to the file: JSON cannot store BigInteger,
        // and all values are recalculated after loading anyway
        [JsonIgnore]
        public object Value { get; set; }

        public Cell(string name)
        {
            Name = name;
            Expression = "";
            Value = "";
        }
    }
}
