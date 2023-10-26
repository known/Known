namespace Known.Cells;

public interface ISheet
{
    public int MaxDataRow { get; }
    public int MaxDataColumn { get; }

    #region Column
    public double GetColumnWidth(int columnIndex);
    public void SetColumnWidth(int columnIndex, double width);
    #endregion

    #region Row
    public int GetFirstRow(string value);
    public double GetRowHeight(int rowIndex);
    public void SetRowHeight(int rowIndex, double height);
    public void SetRowStyle(int rowIndex, StyleInfo info);
    public void SetRowValues(int rowIndex, int startColumn, params object[] args);
    public void InsertRows(int rowIndex, int totalRows);
    #endregion

    #region Cell
    public void SetCellStyle(int rowIndex, int columnIndex, StyleInfo info);
    public void SetCellValue(string cellName, object value, StyleInfo info = null);
    public void SetCellValue(int rowIndex, int columnIndex, object value, StyleInfo info = null);
    public void MergeCells(int firstRow, int firstColumn, int totalRows, int totalColumns);
    #endregion

    #region Import
    public void ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn);
    public void ImportDataByExport(DataTable dataTable);
    #endregion

    #region Image
    public void AddPicture(int upperLeftRow, int upperLeftColumn, int left, int top, int width, int height, string path);
    public void AddPicture(int upperLeftRow, int upperLeftColumn, int left, int top, int width, int height, byte[] bytes);
    #endregion

    #region Replace
    public void Replace(string key, string value);
    public void Replace(int row, int column, string key, object value);
    public void ClearEmpty(string match = "{.?}|{.+}");
    #endregion
}