using System.Data;
using System.IO;

namespace Known.Cells
{
    public interface ISheet
    {
        int Index { get; }
        string Name { get; }
        int ColumnCount { get; }
        int RowCount { get; }

        ISheetCell GetCell(string cellName);
        ISheetCell GetCell(int row, int column);
        void Copy(int sourceSheetIndex);
        void Copy(string sourceSheetName);
        void CopyRange(int sourceFirstRow, int targetFirstRow, int number);
        void CopyRows(int sourceFirstRow, int targetFirstRow, int number);
        void InsertColumn(int columnIndex);
        void InsertColumns(int columnIndex, int totalColumns);
        void InsertRow(int rowIndex);
        void InsertRows(int rowIndex, int totalRows);
        void DeleteColumn(int columnIndex);
        void DeleteRow(int rowIndex);
        void DeleteRows(int rowIndex, int totalRows);
        void HideColumn(int column);
        void HideColumns(int column, int totalColumns);
        void HideRow(int row);
        void HideRows(int row, int totalRows);
        void AutoFitColumns();
        void AutoFitColumns(int firstColumn, int lastColumn);
        void AutoFitRows();
        void AutoFitRows(int startRow, int endRow);
        void Merge(int firstRow, int firstColumn, int totalRows, int totalColumns);
        void UnMerge(int firstRow, int firstColumn, int totalRows, int totalColumns);
        void SetAlignment(int row, int column, TextAlignment align);
        void SetTextAndMerge(int row, int column, int rowCount, int columnCount, string text, TextAlignment align = TextAlignment.Left);
        void SetCellImage(int row, int column, Stream stream, ImageSetting setting = null);
        void SetCellImage(int row, int column, string fileName, ImageSetting setting = null);
        void SetCellBorder(string cellName, bool top, bool right, bool bottom, bool left);
        void SetCellBold(int row, int column);
        void SetColumnWidth(int column, double width);
        void SetRowHeight(int row, double height);
        double GetColumnWidth(int column);
        double GetRowHeight(int row);
        int ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, bool insertRows);
        int ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, int rowNumber, int columnNumber, bool insertRows, string dateFormatString, bool convertStringToNumber);
        DataTable ExportData(int firstRow, int firstColumn, int totalRows, int totalColumns, bool exportColumnName);
        DataTable ExportDataAsString(int firstRow, int firstColumn, int totalRows, int totalColumns, bool exportColumnName);
    }
}
