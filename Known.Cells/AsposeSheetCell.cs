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
        }

        public int Row { get; }
        public int Column { get; }
        public string Name { get; }
        public string StringValue { get; }
        public string DisplayStringValue { get; }

        public string Formula
        {
            get { return cell.Formula; }
            set { cell.Formula = value; }
        }

        public object Value
        {
            get { return cell.Value; }
            set { cell.Value = value; }
        }
    }
}
