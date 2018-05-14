namespace Known.Cells
{
    /// <summary>
    /// Sheet单元格接口。
    /// </summary>
    public interface ISheetCell
    {
        /// <summary>
        /// 取得单元格行号。
        /// </summary>
        int Row { get; }

        /// <summary>
        /// 取得单元格列号。
        /// </summary>
        int Column { get; }

        /// <summary>
        /// 取得单元格名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 取得单元格字符串值。
        /// </summary>
        string StringValue { get; }

        /// <summary>
        /// 取得单元格显示的字符串值。
        /// </summary>
        string DisplayStringValue { get; }

        /// <summary>
        /// 取得单元格的值对象。
        /// </summary>
        object Value { get; }

        /// <summary>
        /// 设置单元格数据。
        /// </summary>
        /// <param name="value">单元格数据。</param>
        void PutValue(object value);
    }
}
