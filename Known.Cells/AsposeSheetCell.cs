using Aspose.Cells;

namespace Known.Cells
{
    /// <summary>
    /// Aspose组件实现的Sheet单元格类。
    /// </summary>
    public class AsposeSheetCell : ISheetCell
    {
        private Cell cell;

        /// <summary>
        /// 构造函数，创建一个Aspose组件实现的Sheet单元格实例。
        /// </summary>
        /// <param name="cell"></param>
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

        /// <summary>
        /// 取得单元格行号。
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// 取得单元格列号。
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// 取得单元格名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 取得单元格字符串值。
        /// </summary>
        public string StringValue { get; }

        /// <summary>
        /// 取得单元格显示的字符串值。
        /// </summary>
        public string DisplayStringValue { get; }

        /// <summary>
        /// 取得单元格的值对象。
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// 设置单元格数据。
        /// </summary>
        /// <param name="value">单元格数据。</param>
        public void PutValue(object value)
        {
            cell.PutValue(value);
        }
    }
}
