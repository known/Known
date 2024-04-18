namespace Known.Cells;

public interface ISheet
{
    int MaxDataRow { get; }
    int MaxDataColumn { get; }

    #region Column
    double GetColumnWidth(int columnIndex);
    void SetColumnWidth(int columnIndex, double width);
    #endregion

    #region Row
    int GetFirstRow(string value);
    double GetRowHeight(int rowIndex);
    void SetRowHeight(int rowIndex, double height);
    void SetRowStyle(int rowIndex, StyleInfo info);
    void SetRowValues(int rowIndex, int startColumn, params object[] args);
    void InsertRows(int rowIndex, int totalRows);
    #endregion

    #region Cell
    void SetCellStyle(int rowIndex, int columnIndex, StyleInfo info);
    void SetCellValue(string cellName, object value, StyleInfo info = null);
    void SetCellValue(int rowIndex, int columnIndex, object value, StyleInfo info = null);
    void MergeCells(int firstRow, int firstColumn, int totalRows, int totalColumns);
    #endregion

    #region Import
    void ImportData(DataTable dataTable);
    void ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn);
    #endregion

    #region Image
    void AddPicture(int upperLeftRow, int upperLeftColumn, int left, int top, int width, int height, string path);
    void AddPicture(int upperLeftRow, int upperLeftColumn, int left, int top, int width, int height, byte[] bytes);
    #endregion

    #region Replace
    void Replace(string key, string value);
    void Replace(int row, int column, string key, object value);
    void ClearEmpty(string match = "{.?}|{.+}");
    #endregion
}