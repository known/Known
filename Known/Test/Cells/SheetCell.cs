using System;

namespace Known.Cells
{
    /// <summary>
    /// 单元格操作类。
    /// </summary>
    public class SheetCell
    {
        private readonly ISheetCell cell;

        internal SheetCell(ISheetCell cell)
        {
            this.cell = cell ?? throw new ArgumentNullException(nameof(cell));
            Row = cell.Row;
            Column = cell.Column;
            Name = cell.Name;
            StringValue = cell.StringValue;
            DisplayStringValue = cell.DisplayStringValue;
        }

        /// <summary>
        /// 取得单元格行序号。
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// 取得单元格列序号。
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// 取得单元格名。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 取得单元格的字符值。
        /// </summary>
        public string StringValue { get; }

        /// <summary>
        /// 取得单元格显示的字符值。
        /// </summary>
        public string DisplayStringValue { get; }

        /// <summary>
        /// 取得或设置单元格的公式。
        /// </summary>
        public string Formula
        {
            get { return cell.Formula; }
            set { cell.Formula = value; }
        }

        /// <summary>
        /// 取得或设置单元格的值。
        /// </summary>
        public object Value
        {
            get { return cell.Value; }
            set { cell.Value = value; }
        }

        /// <summary>
        /// 获取指定泛型的单元格数据。
        /// </summary>
        /// <typeparam name="T">数量类型。</typeparam>
        /// <returns>单元格数据。</returns>
        public T ValueAs<T>()
        {
            return Utils.ConvertTo<T>(Value);
        }
    }
}