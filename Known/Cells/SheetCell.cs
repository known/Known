using System;

namespace Known.Cells
{
    public class SheetCell
    {
        private ISheetCell cell;

        internal SheetCell(ISheetCell cell)
        {
            this.cell = cell ?? throw new ArgumentNullException(nameof(cell));
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

        public object Value
        {
            get { return cell.Value; }
            set { cell.PutValue(value); }
        }

        public T ValueAs<T>()
        {
            return Utils.ConvertTo<T>(Value);
        }
    }
}