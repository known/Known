using Aspose.Cells;

namespace Known.Files
{
    public class SheetCell
    {
        private Cell cell;

        internal SheetCell(Cell cell)
        {
            this.cell = cell;
        }

        public object Value
        {
            get { return cell.Value; }
            set { cell.PutValue(value); }
        }

        public string StringValue
        {
            get { return cell.StringValue; }
        }

        public T ValueAs<T>()
        {
            return Utils.ConvertTo<T>(Value);
        }
    }
}