using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KY.Core.Csv
{
    public class CsvWriter
    {
        private readonly List<CsvRow> rows;

        public string CellSeparator { get; set; } = ";";
        public string RowSeparator { get; set; } = Environment.NewLine;
        public Encoding Encoding { get; set; }

        public CsvWriter()
        {
            this.rows = new List<CsvRow>();
            this.Encoding = Encoding.GetEncoding(1252);
        }

        public CsvRow AddRow()
        {
            CsvRow row = new CsvRow();
            this.rows.Add(row);
            return row;
        }

        public void Write(Stream stream)
        {
            foreach (CsvRow row in this.rows)
            {
                foreach (CsvCell cell in row.Cells)
                {
                    stream.Write(this.Encoding.GetBytes($"\"{cell.Value}\"{this.CellSeparator}"));
                }
                stream.Write(this.Encoding.GetBytes(this.RowSeparator));
            }
        }
    }
}