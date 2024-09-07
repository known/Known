namespace Known.Cells;

/// <summary>
/// Sheet操作接口。
/// </summary>
public interface ISheet
{
    /// <summary>
    /// 取得Sheet最大数据行。
    /// </summary>
    int MaxDataRow { get; }

    /// <summary>
    /// 取得Sheet最大数据列。
    /// </summary>
    int MaxDataColumn { get; }

    #region Column
    /// <summary>
    /// 获取Sheet列宽度。
    /// </summary>
    /// <param name="columnIndex">Sheet列序号。</param>
    /// <returns>Sheet列宽度。</returns>
    double GetColumnWidth(int columnIndex);

    /// <summary>
    /// 设置Sheet列宽度。
    /// </summary>
    /// <param name="columnIndex">Sheet列序号。</param>
    /// <param name="width">Sheet列宽度。</param>
    void SetColumnWidth(int columnIndex, double width);
    #endregion

    #region Row
    /// <summary>
    /// 根据单元格内容获取在Sheet中出现的首行序号。
    /// </summary>
    /// <param name="value">单元格内容字符串。</param>
    /// <returns></returns>
    int GetFirstRow(string value);

    /// <summary>
    /// 获取Sheet行高度。
    /// </summary>
    /// <param name="rowIndex">Sheet行序号。</param>
    /// <returns>Sheet行高度。</returns>
    double GetRowHeight(int rowIndex);

    /// <summary>
    /// 设置Sheet行高度。
    /// </summary>
    /// <param name="rowIndex">Sheet行序号。</param>
    /// <param name="height">Sheet行高度。</param>
    void SetRowHeight(int rowIndex, double height);

    /// <summary>
    /// 设置Sheet行样式。
    /// </summary>
    /// <param name="rowIndex">Sheet行序号。</param>
    /// <param name="info">样式信息对象。</param>
    void SetRowStyle(int rowIndex, StyleInfo info);

    /// <summary>
    /// 设置Sheet行单元格数据。
    /// </summary>
    /// <param name="rowIndex">Sheet行序号。</param>
    /// <param name="startColumn">Sheet开始列序号。</param>
    /// <param name="args">单元格数据。</param>
    void SetRowValues(int rowIndex, int startColumn, params object[] args);

    /// <summary>
    /// 插入Sheet行。
    /// </summary>
    /// <param name="rowIndex">插入Sheet开始行位置序号。</param>
    /// <param name="totalRows">插入行数。</param>
    void InsertRows(int rowIndex, int totalRows);
    #endregion

    #region Cell
    /// <summary>
    /// 根据内容查找对应的单元格信息。
    /// </summary>
    /// <param name="value">单元格内容。</param>
    /// <returns>单元格信息。</returns>
    CellInfo FindCell(object value);

    /// <summary>
    /// 根据单元格名称获取单元格泛型类型值。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="cellName">单元格名称。</param>
    /// <returns>泛型类型值。</returns>
    T GetCellValue<T>(string cellName);

    /// <summary>
    /// 根据单元格行列序号获取单元格泛型类型值。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="rowIndex">单元格行序号。</param>
    /// <param name="columnIndex">单元格列序号。</param>
    /// <returns>泛型类型值。</returns>
    T GetCellValue<T>(int rowIndex, int columnIndex);

    /// <summary>
    /// 根据单元格行列序号设置单元格样式。
    /// </summary>
    /// <param name="rowIndex">单元格行序号。</param>
    /// <param name="columnIndex">单元格列序号。</param>
    /// <param name="info">单元格样式信息。</param>
    void SetCellStyle(int rowIndex, int columnIndex, StyleInfo info);

    /// <summary>
    /// 根据单元格名称设置单元格数据和样式。
    /// </summary>
    /// <param name="cellName">单元格名称。</param>
    /// <param name="value">单元格内容。</param>
    /// <param name="info">单元格样式信息。</param>
    void SetCellValue(string cellName, object value, StyleInfo info = null);

    /// <summary>
    /// 根据单元格行列序号设置单元格数据和样式。
    /// </summary>
    /// <param name="rowIndex">单元格行序号。</param>
    /// <param name="columnIndex">单元格列序号。</param>
    /// <param name="value">单元格内容。</param>
    /// <param name="info">单元格样式信息。</param>
    void SetCellValue(int rowIndex, int columnIndex, object value, StyleInfo info = null);

    /// <summary>
    /// 合并单元格。
    /// </summary>
    /// <param name="firstRow">合并首行序号。</param>
    /// <param name="firstColumn">合并首列序号。</param>
    /// <param name="totalRows">合并总行数。</param>
    /// <param name="totalColumns">合并总列数。</param>
    void MergeCells(int firstRow, int firstColumn, int totalRows, int totalColumns);
    #endregion

    #region Import
    /// <summary>
    /// 将DataTable数据导入到Sheet中。
    /// </summary>
    /// <param name="dataTable">DataTable数据。</param>
    void ImportData(DataTable dataTable);

    /// <summary>
    /// 将DataTable数据导入到Sheet中。
    /// </summary>
    /// <param name="dataTable">DataTable数据</param>
    /// <param name="isFieldNameShown">是否显示字段名称。</param>
    /// <param name="firstRow">导入单元格第一行序号。</param>
    /// <param name="firstColumn">导入单元格第一列序号。</param>
    void ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn);
    #endregion

    #region Image
    /// <summary>
    /// 将图片插入到Sheet中的指定位置。
    /// </summary>
    /// <param name="upperLeftRow">图片左上角行序号。</param>
    /// <param name="upperLeftColumn">图片左上角列序号。</param>
    /// <param name="left">图片左边距离。</param>
    /// <param name="top">图片上边距离。</param>
    /// <param name="width">图片宽度。</param>
    /// <param name="height">图片高度。</param>
    /// <param name="path">图片文件路径。</param>
    void AddPicture(int upperLeftRow, int upperLeftColumn, int left, int top, int width, int height, string path);

    /// <summary>
    /// 将图片插入到Sheet中的指定位置。
    /// </summary>
    /// <param name="upperLeftRow">图片左上角行序号。</param>
    /// <param name="upperLeftColumn">图片左上角列序号。</param>
    /// <param name="left">图片左边距离。</param>
    /// <param name="top">图片上边距离。</param>
    /// <param name="width">图片宽度。</param>
    /// <param name="height">图片高度。</param>
    /// <param name="bytes">图片文件字节数组。</param>
    void AddPicture(int upperLeftRow, int upperLeftColumn, int left, int top, int width, int height, byte[] bytes);
    #endregion

    #region Replace
    /// <summary>
    /// 替换整个Sheet中指定的字符。
    /// </summary>
    /// <param name="key">要替换的字符。</param>
    /// <param name="value">替换后的字符。</param>
    void Replace(string key, string value);

    /// <summary>
    /// 替换Sheet单元格指定的字符。
    /// </summary>
    /// <param name="row">单元格行序号。</param>
    /// <param name="column">单元格列序号。</param>
    /// <param name="key">要替换的字符。</param>
    /// <param name="value">替换后的字符。</param>
    void Replace(int row, int column, string key, object value);

    /// <summary>
    /// 删除Sheet中的空白行。
    /// </summary>
    /// <param name="match"></param>
    void ClearEmpty(string match = "{.?}|{.+}");
    #endregion
}