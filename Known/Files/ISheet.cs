using System.Data;
using System.IO;

namespace Known.Files
{
    /// <summary>
    /// Sheet接口。
    /// </summary>
    public interface ISheet
    {
        /// <summary>
        /// 取得Excel中Sheet的索引位置。
        /// </summary>
        int Index { get; }

        /// <summary>
        /// 取得Excel中Sheet的名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 取得Excel中Sheet的数据列数。
        /// </summary>
        int ColumnCount { get; }

        /// <summary>
        /// 取得Excel中Sheet的数据行数。
        /// </summary>
        int RowCount { get; }

        /// <summary>
        /// 根据Sheet单元格名称获取Sheet单元格对象。
        /// </summary>
        /// <param name="cellName">单元格名称。</param>
        /// <returns>Sheet单元格对象。</returns>
        ISheetCell GetCell(string cellName);

        /// <summary>
        /// 根据Sheet单元格行列号获取Sheet单元格对象。
        /// </summary>
        /// <param name="row">单元格行号。</param>
        /// <param name="column">单元格列号。</param>
        /// <returns>Sheet单元格对象。</returns>
        ISheetCell GetCell(int row, int column);

        /// <summary>
        /// 复制整个Sheet。
        /// </summary>
        /// <param name="sourceSheetIndex">源Sheet索引位置。</param>
        void Copy(int sourceSheetIndex);

        /// <summary>
        /// 复制整个Sheet。
        /// </summary>
        /// <param name="sourceSheetName">源Sheet名称。</param>
        void Copy(string sourceSheetName);

        /// <summary>
        /// 复制单元格范围。
        /// </summary>
        /// <param name="sourceFirstRow">原Sheet开始行号。</param>
        /// <param name="targetFirstRow">目标Sheet开始行号。</param>
        /// <param name="number">要复制的行数。</param>
        void CopyRange(int sourceFirstRow, int targetFirstRow, int number);

        /// <summary>
        /// 复制数据行。
        /// </summary>
        /// <param name="sourceFirstRow">原Sheet开始行号。</param>
        /// <param name="targetFirstRow">目标Sheet开始行号。</param>
        /// <param name="number">要复制的行数。</param>
        void CopyRows(int sourceFirstRow, int targetFirstRow, int number);

        /// <summary>
        /// 插入Sheet列。
        /// </summary>
        /// <param name="columnIndex">插入列的索引位置。</param>
        void InsertColumn(int columnIndex);

        /// <summary>
        /// 插入多个Sheet列。
        /// </summary>
        /// <param name="columnIndex">插入列的索引位置。</param>
        /// <param name="totalColumns">插入的列数。</param>
        void InsertColumns(int columnIndex, int totalColumns);

        /// <summary>
        /// 插入Sheet行。
        /// </summary>
        /// <param name="rowIndex">插入行的索引位置。</param>
        void InsertRow(int rowIndex);

        /// <summary>
        /// 插入多个Sheet行。
        /// </summary>
        /// <param name="rowIndex">插入行的索引位置。</param>
        /// <param name="totalRows">插入的行数。</param>
        void InsertRows(int rowIndex, int totalRows);

        /// <summary>
        /// 删除Sheet列。
        /// </summary>
        /// <param name="columnIndex">删除列的索引位置。</param>
        void DeleteColumn(int columnIndex);

        /// <summary>
        /// 删除Sheet行。
        /// </summary>
        /// <param name="rowIndex">删除行的索引位置。</param>
        void DeleteRow(int rowIndex);

        /// <summary>
        /// 删除多个Sheet行。
        /// </summary>
        /// <param name="rowIndex">删除列的索引位置。</param>
        /// <param name="totalRows">删除的行数。</param>
        void DeleteRows(int rowIndex, int totalRows);

        /// <summary>
        /// 隐藏Sheet列。
        /// </summary>
        /// <param name="column">隐藏列的索引位置。</param>
        void HideColumn(int column);

        /// <summary>
        /// 隐藏多个Sheet列。
        /// </summary>
        /// <param name="column">隐藏列的索引位置。</param>
        /// <param name="totalColumns">隐藏的列数。</param>
        void HideColumns(int column, int totalColumns);

        /// <summary>
        /// 隐藏Sheet行。
        /// </summary>
        /// <param name="row">隐藏行的索引位置。</param>
        void HideRow(int row);

        /// <summary>
        /// 隐藏多个Sheet行。
        /// </summary>
        /// <param name="row">隐藏行的索引位置。</param>
        /// <param name="totalRows">隐藏的行数。</param>
        void HideRows(int row, int totalRows);

        /// <summary>
        /// 自动调整所有列的宽度。
        /// </summary>
        void AutoFitColumns();

        /// <summary>
        /// 自动调整所有行的高度。
        /// </summary>
        void AutoFitRows();

        /// <summary>
        /// 合并单元格。
        /// </summary>
        /// <param name="firstRow">开始合并行号。</param>
        /// <param name="firstColumn">开始合并列号。</param>
        /// <param name="totalRows">合并的总行数。</param>
        /// <param name="totalColumns">合并的总列数。</param>
        void Merge(int firstRow, int firstColumn, int totalRows, int totalColumns);

        /// <summary>
        /// 取消合并单元格。
        /// </summary>
        /// <param name="firstRow">开始合并行号。</param>
        /// <param name="firstColumn">开始合并列号。</param>
        /// <param name="totalRows">合并的总行数。</param>
        /// <param name="totalColumns">合并的总列数。</param>
        void UnMerge(int firstRow, int firstColumn, int totalRows, int totalColumns);

        /// <summary>
        /// 设置单元格对齐方式。
        /// </summary>
        /// <param name="row">单元格行号。</param>
        /// <param name="column">单元格列号。</param>
        /// <param name="align">对齐方式。</param>
        void SetAlignment(int row, int column, TextAlignment align);

        /// <summary>
        /// 设置单元格文本并合并。
        /// </summary>
        /// <param name="row">单元格行号。</param>
        /// <param name="column">单元格列号。</param>
        /// <param name="rowCount">合并的行数。</param>
        /// <param name="columnCount">合并的列数。</param>
        /// <param name="text">设置的文本。</param>
        /// <param name="align">文本的对齐方式。</param>
        void SetTextAndMerge(int row, int column, int rowCount, int columnCount, string text, TextAlignment align = TextAlignment.Left);

        /// <summary>
        /// 设置单元格图片。
        /// </summary>
        /// <param name="row">单元格行号。</param>
        /// <param name="column">单元格列号。</param>
        /// <param name="stream">图片文件流。</param>
        void SetCellImage(int row, int column, Stream stream);

        /// <summary>
        /// 设置单元格图片。
        /// </summary>
        /// <param name="row">单元格行号。</param>
        /// <param name="column">单元格列号。</param>
        /// <param name="fileName">图片文件路径。</param>
        void SetCellImage(int row, int column, string fileName);

        /// <summary>
        /// 设置单元格边框。
        /// </summary>
        /// <param name="cellName">单元格名。</param>
        /// <param name="top">是否设置上边框。</param>
        /// <param name="right">是否设置右边框。</param>
        /// <param name="bottom">是否设置下边框。</param>
        /// <param name="left">是否设置左边框。</param>
        void SetCellBorder(string cellName, bool top, bool right, bool bottom, bool left);

        /// <summary>
        /// 设置单元格粗体。
        /// </summary>
        /// <param name="row">单元格行号。</param>
        /// <param name="column">单元格列号。</param>
        void SetCellBold(int row, int column);

        /// <summary>
        /// 设置Sheet列宽度。
        /// </summary>
        /// <param name="column">列的索引位置。</param>
        /// <param name="width">列的宽度。</param>
        void SetColumnWidth(int column, double width);

        /// <summary>
        /// 设置Sheet行高度。
        /// </summary>
        /// <param name="row">行的索引位置。</param>
        /// <param name="height">行的高度。</param>
        void SetRowHeight(int row, double height);

        /// <summary>
        /// 获取列的宽度。
        /// </summary>
        /// <param name="column">列的索引位置。</param>
        /// <returns>列的宽度。</returns>
        double GetColumnWidth(int column);

        /// <summary>
        /// 获取行的高度。
        /// </summary>
        /// <param name="row">行的索引位置。</param>
        /// <returns>行的高度。</returns>
        double GetRowHeight(int row);

        /// <summary>
        /// 将DataTable导入到Sheet中。
        /// </summary>
        /// <param name="dataTable">导入的数据。</param>
        /// <param name="isFieldNameShown">是否显示数据列名。</param>
        /// <param name="firstRow">导入的首行位置。</param>
        /// <param name="firstColumn">导入的首列位置。</param>
        /// <param name="insertRows">是否插入行。</param>
        /// <returns>导入的行数。</returns>
        int ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, bool insertRows);

        /// <summary>
        /// 将DataTable导入到Sheet中。
        /// </summary>
        /// <param name="dataTable">导入的数据。</param>
        /// <param name="isFieldNameShown">是否显示数据列名。</param>
        /// <param name="firstRow">导入的首行位置。</param>
        /// <param name="firstColumn">导入的首列位置。</param>
        /// <param name="rowNumber">导入的行数。</param>
        /// <param name="columnNumber">导入的列数。</param>
        /// <param name="insertRows">是否插入行。</param>
        /// <param name="dateFormatString">日期类型的格式。</param>
        /// <param name="convertStringToNumber">是否转换字符串为数值。</param>
        /// <returns>导入的行数。</returns>
        int ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, int rowNumber, int columnNumber, bool insertRows, string dateFormatString, bool convertStringToNumber);

        /// <summary>
        /// 导出Sheet中的数据。
        /// </summary>
        /// <param name="firstRow">导出的首行位置。</param>
        /// <param name="firstColumn">导出的首列位置。</param>
        /// <param name="totalRows">导出的行数。</param>
        /// <param name="totalColumns">导出的列数。</param>
        /// <param name="exportColumnName">是否导出列名。</param>
        /// <returns>导出的数据。</returns>
        DataTable ExportData(int firstRow, int firstColumn, int totalRows, int totalColumns, bool exportColumnName);

        /// <summary>
        /// 以字符串的形式导出Sheet中的数据。
        /// </summary>
        /// <param name="firstRow">导出的首行位置。</param>
        /// <param name="firstColumn">导出的首列位置。</param>
        /// <param name="totalRows">导出的行数。</param>
        /// <param name="totalColumns">导出的列数。</param>
        /// <param name="exportColumnName">是否导出列名。</param>
        /// <returns>导出的数据。</returns>
        DataTable ExportDataAsString(int firstRow, int firstColumn, int totalRows, int totalColumns, bool exportColumnName);
    }
}
