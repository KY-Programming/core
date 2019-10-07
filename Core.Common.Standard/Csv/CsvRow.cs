using System.Collections.Generic;

namespace KY.Core.Csv
{
    public class CsvRow
    {
        public List<CsvCell> Cells { get; }

        public CsvRow()
        {
            this.Cells = new List<CsvCell>();
        }

        public CsvRow AddCell(object value)
        {
            CsvCell cell = new CsvCell(value);
            this.Cells.Add(cell);
            return this;
        }
    }
}