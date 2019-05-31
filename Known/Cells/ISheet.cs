using System.Data;
using System.IO;

namespace Known.Cells
{
    /// <summary>
    /// Sheet 页操作接口。
    /// </summary>
    public interface ISheet
    {
        /// <summary>
        /// 取得 Sheet 页的索引序号。
        /// </summary>
        int Index { get; }

        /// <summary>
        /// 取得 Sheet 页的名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 取得 Sheet 页的数据列数。
        /// </summary>
        int ColumnCount { get; }

        /// <summary>
        /// 取得 Sheet 页的数据行数。
        /// </summary>
        int RowCount { get; }

        /// <summary>
        /// 根据名称获取单元格对象。
        /// </summary>
        /// <param name="cellName">单元格名称。</param>
        /// <returns>单元格对象。</returns>
        ISheetCell GetCell(string cellName);

        /// <summary>
        /// 根据行列序号获取单元格对象。
        /// </summary>
        /// <param name="row">行号。</param>
        /// <param name="column">列号。</param>
        /// <returns>单元格对象。</returns>
        ISheetCell GetCell(int row, int column);

        /// <summary>
        /// 复制 Excel 中的指定序号的 Sheet 页。
        /// </summary>
        /// <param name="sourceIndex">源 Sheet 页序号。</param>
        void Copy(int sourceIndex);

        /// <summary>
        /// 复制 Excel 中的指定名称的 Sheet 页。
        /// </summary>
        /// <param name="sourceName">源 Sheet 页名称。</param>
        void Copy(string sourceName);

        /// <summary>
        /// 复制指定范围的单元格。
        /// </summary>
        /// <param name="sourceFirstIndex">源首行/列号。</param>
        /// <param name="targetFirstIndex">目的首行/列号。</param>
        /// <param name="number">行/列数。</param>
        void CopyRange(int sourceFirstIndex, int targetFirstIndex, int number);

        /// <summary>
        /// 复制指定范围的行。
        /// </summary>
        /// <param name="sourceFirstRow">源首行号。</param>
        /// <param name="targetFirstRow">目的首行号。</param>
        /// <param name="number">行数。</param>
        void CopyRows(int sourceFirstRow, int targetFirstRow, int number);

        /// <summary>
        /// 在指定列序号位置插入一个新列。
        /// </summary>
        /// <param name="columnIndex">列序号。</param>
        void InsertColumn(int columnIndex);

        /// <summary>
        /// 在指定列序号位置插入多个新列。
        /// </summary>
        /// <param name="columnIndex">列序号。</param>
        /// <param name="totalColumns">插入的列数。</param>
        void InsertColumns(int columnIndex, int totalColumns);

        /// <summary>
        /// 在指定行序号位置插入一个新行。
        /// </summary>
        /// <param name="rowIndex">行序号。</param>
        void InsertRow(int rowIndex);

        /// <summary>
        /// 在指定行序号位置插入多个新行。
        /// </summary>
        /// <param name="rowIndex">行序号。</param>
        /// <param name="totalRows">插入的行数。</param>
        void InsertRows(int rowIndex, int totalRows);

        /// <summary>
        /// 删除指定序号的列。
        /// </summary>
        /// <param name="columnIndex">列序号。</param>
        void DeleteColumn(int columnIndex);

        /// <summary>
        /// 删除指定序号的行。
        /// </summary>
        /// <param name="rowIndex">行序号。</param>
        void DeleteRow(int rowIndex);

        /// <summary>
        /// 删除指定序号位置的多行。
        /// </summary>
        /// <param name="rowIndex">行序号。</param>
        /// <param name="totalRows">删除的行数。</param>
        void DeleteRows(int rowIndex, int totalRows);

        /// <summary>
        /// 隐藏指定序号的列。
        /// </summary>
        /// <param name="column">列序号。</param>
        void HideColumn(int column);

        /// <summary>
        /// 隐藏指定序号位置的多列。
        /// </summary>
        /// <param name="column">列序号。</param>
        /// <param name="totalColumns">隐藏的列数。</param>
        void HideColumns(int column, int totalColumns);

        /// <summary>
        /// 隐藏指定序号的行。
        /// </summary>
        /// <param name="row">行序号。</param>
        void HideRow(int row);

        /// <summary>
        /// 隐藏指定序号位置的多行。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="totalRows">隐藏的行数。</param>
        void HideRows(int row, int totalRows);

        /// <summary>
        /// 自动调整指定列的宽度。
        /// </summary>
        /// <param name="column">列序号。</param>
        void AutoFitColumn(int column);

        /// <summary>
        /// 自动调整所有数据列的宽度。
        /// </summary>
        void AutoFitColumns();

        /// <summary>
        /// 自动调整指定范围列的宽度。
        /// </summary>
        /// <param name="firstColumn">首列序号。</param>
        /// <param name="lastColumn">尾列序号。</param>
        void AutoFitColumns(int firstColumn, int lastColumn);

        /// <summary>
        /// 自动调整指定行的高度。
        /// </summary>
        /// <param name="row">行序号。</param>
        void AutoFitRow(int row);

        /// <summary>
        /// 自动调整所有数据行的高度。
        /// </summary>
        void AutoFitRows();

        /// <summary>
        /// 自动调整指定范围行的高度。
        /// </summary>
        /// <param name="startRow">开始行序号。</param>
        /// <param name="endRow">结束行序号。</param>
        void AutoFitRows(int startRow, int endRow);

        /// <summary>
        /// 合并指定范围的单元格。
        /// </summary>
        /// <param name="firstRow">首行序号。</param>
        /// <param name="firstColumn">首列序号。</param>
        /// <param name="totalRows">总行数。</param>
        /// <param name="totalColumns">总列数。</param>
        void Merge(int firstRow, int firstColumn, int totalRows, int totalColumns);

        /// <summary>
        /// 不合并指定范围的单元格。
        /// </summary>
        /// <param name="firstRow">首行序号。</param>
        /// <param name="firstColumn">首列序号。</param>
        /// <param name="totalRows">总行数。</param>
        /// <param name="totalColumns">总列数。</param>
        void UnMerge(int firstRow, int firstColumn, int totalRows, int totalColumns);

        /// <summary>
        /// 设置指定单元格的内容对齐方式。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="column">列序号。</param>
        /// <param name="align">对齐方式。</param>
        void SetAlignment(int row, int column, TextAlignment align);

        /// <summary>
        /// 设置指定单元格文本合并和对齐方式。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="column">列序号。</param>
        /// <param name="rowCount">合并列数。</param>
        /// <param name="columnCount">合并行数。</param>
        /// <param name="text">文本内容。</param>
        /// <param name="align">对齐方式，默认左对齐。</param>
        void SetTextAndMerge(int row, int column, int rowCount, int columnCount, string text, TextAlignment align = TextAlignment.Left);

        /// <summary>
        /// 设置指定单元格的图片。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="column">列序号。</param>
        /// <param name="stream">图片内容流。</param>
        /// <param name="setting">图片位置及大小设置。</param>
        void SetCellImage(int row, int column, Stream stream, ImageSetting setting = null);

        /// <summary>
        /// 设置指定单元格的图片。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="column">列序号。</param>
        /// <param name="fileName">图片文件路径。</param>
        /// <param name="setting">图片位置及大小设置。</param>
        void SetCellImage(int row, int column, string fileName, ImageSetting setting = null);

        /// <summary>
        /// 设置指定单元格的边框。
        /// </summary>
        /// <param name="cellName">单元格名。</param>
        /// <param name="top">是否有上边框。</param>
        /// <param name="right">是否有右边框。</param>
        /// <param name="bottom">是否有下边框。</param>
        /// <param name="left">是否有左边框。</param>
        void SetCellBorder(string cellName, bool top, bool right, bool bottom, bool left);

        /// <summary>
        /// 设置指定单元格为粗体。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="column">列序号。</param>
        void SetCellBold(int row, int column);

        /// <summary>
        /// 设置指定列表的宽度。
        /// </summary>
        /// <param name="column">列序号。</param>
        /// <param name="width">宽度。</param>
        void SetColumnWidth(int column, double width);

        /// <summary>
        /// 设置指定行的高度。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="height">高度。</param>
        void SetRowHeight(int row, double height);

        /// <summary>
        /// 获取指定列的宽度。
        /// </summary>
        /// <param name="column">列序号。</param>
        /// <returns>列宽度。</returns>
        double GetColumnWidth(int column);

        /// <summary>
        /// 获取指定行的高度。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <returns>行高度。</returns>
        double GetRowHeight(int row);

        /// <summary>
        /// 将数据表导入到 Sheet 页的指定位置。
        /// </summary>
        /// <param name="dataTable">数据表。</param>
        /// <param name="isFieldNameShown">是否显示列名。</param>
        /// <param name="firstRow">导入首行序号。</param>
        /// <param name="firstColumn">导入首列序号。</param>
        /// <param name="insertRows">是否在指定插入行。</param>
        /// <returns>导入的行数。</returns>
        int ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, bool insertRows);

        /// <summary>
        /// 将数据表导入到 Sheet 页的指定位置。
        /// </summary>
        /// <param name="dataTable">数据表。</param>
        /// <param name="isFieldNameShown">是否显示列名。</param>
        /// <param name="firstRow">导入首行序号。</param>
        /// <param name="firstColumn">导入首列序号。</param>
        /// <param name="rowNumber">总行数。</param>
        /// <param name="columnNumber">总列数。</param>
        /// <param name="insertRows">是否在指定插入行。</param>
        /// <param name="dateFormatString">日期格式。</param>
        /// <param name="convertStringToNumber">是否将字符转成数字或日期。</param>
        /// <returns>导入的行数。</returns>
        int ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, int rowNumber, int columnNumber, bool insertRows, string dateFormatString, bool convertStringToNumber);

        /// <summary>
        /// 导出指定位置的 Sheet 页数据。
        /// </summary>
        /// <param name="firstRow">导出首行序号。</param>
        /// <param name="firstColumn">导出首列序号。</param>
        /// <param name="totalRows">总行数。</param>
        /// <param name="totalColumns">总列数。</param>
        /// <param name="exportColumnName">是否导出列名。</param>
        /// <returns>数据表。</returns>
        DataTable ExportData(int firstRow, int firstColumn, int totalRows, int totalColumns, bool exportColumnName);

        /// <summary>
        /// 以字符格式导出指定位置的 Sheet 页数据。
        /// </summary>
        /// <param name="firstRow">导出首行序号。</param>
        /// <param name="firstColumn">导出首列序号。</param>
        /// <param name="totalRows">总行数。</param>
        /// <param name="totalColumns">总列数。</param>
        /// <param name="exportColumnName">是否导出列名。</param>
        /// <returns>数据表。</returns>
        DataTable ExportDataAsString(int firstRow, int firstColumn, int totalRows, int totalColumns, bool exportColumnName);
    }
}
