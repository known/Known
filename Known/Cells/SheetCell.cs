namespace Known.Cells
{
    /// <summary>
    /// Sheet单元格类。
    /// </summary>
    public class SheetCell
    {
        private ISheetCell cell;

        /// <summary>
        /// 构造函数，创建一个Sheet单元格实例。
        /// </summary>
        /// <param name="cell">单元格接口。</param>
        internal SheetCell(ISheetCell cell)
        {
            this.cell = cell;
            StringValue = cell.StringValue;
            DisplayStringValue = cell.DisplayStringValue;
        }

        /// <summary>
        /// 取得单元格字符串值。
        /// </summary>
        public string StringValue { get; }

        /// <summary>
        /// 取得单元格显示的字符串值。
        /// </summary>
        public string DisplayStringValue { get; }

        /// <summary>
        /// 取得或设置单元格数据。
        /// </summary>
        public object Value
        {
            get { return cell.Value; }
            set { cell.PutValue(value); }
        }

        /// <summary>
        /// 获取单元格指定类型的数据。
        /// </summary>
        /// <typeparam name="T">数据的类型。</typeparam>
        /// <returns>指定类型的数据。</returns>
        public T ValueAs<T>()
        {
            return Utils.ConvertTo<T>(Value);
        }
    }
}