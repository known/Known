using Aspose.Cells;

namespace Known.Cells
{
    public class AsposeSheetCell : ISheetCell
    {
        private Cell cell;

        public AsposeSheetCell(Cell cell)
        {
            this.cell = cell;

            Row = cell.Row;
            Column = cell.Column;
            Name = cell.Name;
            StringValue = cell.StringValue;
            DisplayStringValue = cell.DisplayStringValue;
            Value = cell.Value;
        }

        public int Row { get; }
        public int Column { get; }
        public string Name { get; }
        public string StringValue { get; }
        public string DisplayStringValue { get; }
        public object Value { get; }

        public void PutValue(object value)
        {
            cell.PutValue(value);
        }
    }
}
