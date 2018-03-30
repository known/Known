namespace Known.Files
{
    public class SheetCell
    {
        private ISheetCell cell;

        internal SheetCell(ISheetCell cell)
        {
            this.cell = cell;
            StringValue = cell.StringValue;
            DisplayStringValue = cell.DisplayStringValue;
        }

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