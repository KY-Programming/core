namespace KY.Core.Csv
{
    public class CsvCell
    {
        public object Value { get; }

        public CsvCell(object value)
        {
            this.Value = value;
        }
    }
}