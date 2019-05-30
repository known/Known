namespace Known.Cells
{
    /// <summary>
    /// 单元格操作接口。
    /// </summary>
    public interface ISheetCell
    {
        /// <summary>
        /// 取得单元格行序号。
        /// </summary>
        int Row { get; }

        /// <summary>
        /// 取得单元格列序号。
        /// </summary>
        int Column { get; }

        /// <summary>
        /// 取得单元格名。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 取得单元格的字符值。
        /// </summary>
        string StringValue { get; }

        /// <summary>
        /// 取得单元格显示的字符值。
        /// </summary>
        string DisplayStringValue { get; }

        /// <summary>
        /// 取得或设置单元格的公式。
        /// </summary>
        string Formula { get; set; }

        /// <summary>
        /// 取得或设置单元格的值。
        /// </summary>
        object Value { get; set; }
    }
}
