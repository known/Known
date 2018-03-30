using Aspose.Cells;
using System.Data;
using System.Drawing;
using System.IO;

namespace Known.Files
{
    public class AsposeSheet : ISheet
    {
        private Worksheet sheet;

        public AsposeSheet(Workbook wb, string name)
        {
            sheet = wb.Worksheets[name];
            Index = sheet.Index;
            Name = sheet.Name;
            ColumnCount = sheet.Cells.MaxDataColumn + 1;
            RowCount = sheet.Cells.MaxDataRow + 1;
        }

        public int Index { get; }
        public string Name { get; }
        public int ColumnCount { get; }
        public int RowCount { get; }

        public ISheetCell GetCell(string cellName)
        {
            var cell = sheet.Cells[cellName];
            return new AsposeSheetCell(cell);
        }

        public ISheetCell GetCell(int row, int column)
        {
            var cell = sheet.Cells[row, column];
            return new AsposeSheetCell(cell);
        }

        public void Copy(int sourceSheetIndex)
        {
            var sourceSheet = sheet.Workbook.Worksheets[sourceSheetIndex];
            sheet.Copy(sourceSheet);
        }

        public void Copy(string sourceSheetName)
        {
            var sourceSheet = sheet.Workbook.Worksheets[sourceSheetName];
            sheet.Copy(sourceSheet);
        }

        public void CopyRange(int sourceFirstRow, int targetFirstRow, int number)
        {
            var sourceRows = sheet.Cells.CreateRange(sourceFirstRow, number, false);
            var targetRows = sheet.Cells.CreateRange(targetFirstRow, number, false);
            targetRows.Copy(sourceRows);
            for (int i = 0; i < number; i++)
            {
                SetRowHeight(targetFirstRow + i, GetRowHeight(i));
            }
        }

        public void CopyRows(int sourceFirstRow, int targetFirstRow, int number)
        {
            sheet.Cells.CopyRows(sheet.Cells, sourceFirstRow, targetFirstRow, number);
        }

        public void InsertColumn(int columnIndex)
        {
            sheet.Cells.InsertColumn(columnIndex);
        }

        public void InsertColumns(int columnIndex, int totalColumns)
        {
            sheet.Cells.InsertColumns(columnIndex, totalColumns);
        }

        public void InsertRow(int rowIndex)
        {
            sheet.Cells.InsertRow(rowIndex);
        }

        public void InsertRows(int rowIndex, int totalRows)
        {
            sheet.Cells.InsertRows(rowIndex, totalRows);
        }

        public void DeleteColumn(int columnIndex)
        {
            sheet.Cells.DeleteColumn(columnIndex);
        }

        public void DeleteRow(int rowIndex)
        {
            sheet.Cells.DeleteRow(rowIndex);
        }

        public void DeleteRows(int rowIndex, int totalRows)
        {
            sheet.Cells.DeleteRows(rowIndex, totalRows);
        }

        public void AutoFitColumns()
        {
            sheet.AutoFitColumns();
        }

        public void AutoFitRows()
        {
            sheet.AutoFitRows();
        }

        public void Merge(int firstRow, int firstColumn, int totalRows, int totalColumns)
        {
            sheet.Cells.Merge(firstRow, firstColumn, totalRows, totalColumns);
        }

        public void UnMerge(int firstRow, int firstColumn, int totalRows, int totalColumns)
        {
            sheet.Cells.UnMerge(firstRow, firstColumn, totalRows, totalColumns);
        }

        public void HideColumn(int column)
        {
            sheet.Cells.HideColumn(column);
        }

        public void HideColumns(int column, int totalColumns)
        {
            sheet.Cells.HideColumns(column, totalColumns);
        }

        public void HideRow(int row)
        {
            sheet.Cells.HideRow(row);
        }

        public void HideRows(int row, int totalRows)
        {
            sheet.Cells.HideRows(row, totalRows);
        }

        public void SetAlignment(int row, int column, TextAlignment align)
        {
            var cell = sheet.Cells[row, column];
            var style = cell.GetStyle();
            if (style == null)
                style = new CellsFactory().CreateStyle();

            switch (align)
            {
                case TextAlignment.Center:
                    style.HorizontalAlignment = TextAlignmentType.Center;
                    style.VerticalAlignment = TextAlignmentType.Center;
                    break;
                case TextAlignment.Left:
                    style.HorizontalAlignment = TextAlignmentType.Left;
                    break;
                case TextAlignment.Right:
                    style.HorizontalAlignment = TextAlignmentType.Right;
                    break;
                case TextAlignment.Top:
                    style.VerticalAlignment = TextAlignmentType.Top;
                    break;
                case TextAlignment.Bottom:
                    style.VerticalAlignment = TextAlignmentType.Bottom;
                    break;
                default:
                    break;
            }
            cell.SetStyle(style);
        }

        public void SetTextAndMerge(int row, int column, int rowCount, int columnCount, string text, TextAlignment align = TextAlignment.Left)
        {
            sheet.Cells.Merge(row, column, rowCount, columnCount);
            sheet.Cells[row, column].Value = text;
            var style = new CellsFactory().CreateStyle();
            style.Font.IsBold = true;
            if (align == TextAlignment.Left)
                style.HorizontalAlignment = TextAlignmentType.Left;
            else if (align == TextAlignment.Center)
                style.HorizontalAlignment = TextAlignmentType.Center;
            else if (align == TextAlignment.Right)
                style.HorizontalAlignment = TextAlignmentType.Right;
            style.VerticalAlignment = TextAlignmentType.Center;
            sheet.Cells[row, column].SetStyle(style);
        }

        public void SetCellImage(int row, int column, Stream stream)
        {
            var index = sheet.Pictures.Add(row, column, stream);
            sheet.Pictures[index].Left = 10;
            sheet.Pictures[index].Top = 10;
        }

        public void SetCellImage(int row, int column, string fileName)
        {
            var index = sheet.Pictures.Add(row, column, fileName);
            sheet.Pictures[index].Left = 10;
            sheet.Pictures[index].Top = 10;
        }

        public void SetCellBorder(string cellName, bool top, bool right, bool bottom, bool left)
        {
            var style = new CellsFactory().CreateStyle();
            if (top)
            {
                style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style.Borders[BorderType.TopBorder].Color = Color.Black;
            }
            if (right)
            {
                style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style.Borders[BorderType.RightBorder].Color = Color.Black;
            }
            if (bottom)
            {
                style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                style.Borders[BorderType.BottomBorder].Color = Color.Black;
            }
            if (left)
            {
                style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style.Borders[BorderType.LeftBorder].Color = Color.Black;
            }
            sheet.Cells[cellName].SetStyle(style);
        }

        public void SetCellBold(int row, int column)
        {
            var style = new CellsFactory().CreateStyle();
            style.Font.IsBold = true;
            sheet.Cells[row, column].SetStyle(style);
        }

        public void SetColumnWidth(int column, double width)
        {
            sheet.Cells.SetColumnWidth(column, width);
        }

        public void SetRowHeight(int row, double height)
        {
            sheet.Cells.SetRowHeight(row, height);
        }

        public double GetColumnWidth(int column)
        {
            return sheet.Cells.GetColumnWidth(column);
        }

        public double GetRowHeight(int row)
        {
            return sheet.Cells.GetRowHeight(row);
        }

        public int ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, bool insertRows)
        {
            return sheet.Cells.ImportDataTable(dataTable, isFieldNameShown, firstRow, firstColumn, insertRows);
        }

        public int ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, int rowNumber, int columnNumber, bool insertRows, string dateFormatString, bool convertStringToNumber)
        {
            return sheet.Cells.ImportDataTable(dataTable, isFieldNameShown, firstRow, firstColumn, rowNumber, columnNumber, insertRows, dateFormatString, convertStringToNumber);
        }

        public DataTable ExportData(int firstRow, int firstColumn, int totalRows, int totalColumns, bool exportColumnName)
        {
            return sheet.Cells.ExportDataTableAsString(firstRow, firstColumn, totalRows, totalColumns, exportColumnName);
        }

        public DataTable ExportDataAsString(int firstRow, int firstColumn, int totalRows, int totalColumns, bool exportColumnName)
        {
            return sheet.Cells.ExportDataTableAsString(firstRow, firstColumn, totalRows, totalColumns, exportColumnName);
        }
    }
}
