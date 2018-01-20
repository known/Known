using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;

namespace Known.Files
{
    public class Sheet
    {
        private DataColumnCollection columns;

        internal Sheet(Workbook wb, string name)
        {
            if (wb == null)
                throw new ArgumentNullException("wb");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            InnerSheet = wb.Worksheets[name];
            Name = name;
        }

        internal Worksheet InnerSheet { get; private set; }

        public int Index
        {
            get { return InnerSheet.Index; }
        }

        public string Name
        {
            get { return InnerSheet.Name; }
            set { InnerSheet.Name = value; }
        }

        public int ColumnCount
        {
            get { return InnerSheet.Cells.MaxDataColumn + 1; }
        }

        public int RowCount
        {
            get { return InnerSheet.Cells.MaxDataRow + 1; }
        }

        public void SetModuleName(int row, int column, string moduleName)
        {
            InnerSheet.Cells.Merge(row, column, 1, ColumnCount);
            this[row, column].Value = moduleName;
            var style = new CellsFactory().CreateStyle();
            style.Font.IsBold = true;
            style.HorizontalAlignment = TextAlignmentType.Center;
            style.VerticalAlignment = TextAlignmentType.Center;
            InnerSheet.Cells[row, column].SetStyle(style);
        }

        public void SetColumnHeader(int row, int column, int rowCount, int columnCount, string header, string align = "left")
        {
            InnerSheet.Cells.Merge(row, column, rowCount, columnCount);
            this[row, column].Value = header;
            var style = new CellsFactory().CreateStyle();
            style.Font.IsBold = true;
            if (align == "left")
                style.HorizontalAlignment = TextAlignmentType.Left;
            else if (align == "center")
                style.HorizontalAlignment = TextAlignmentType.Center;
            else if (align == "right")
                style.HorizontalAlignment = TextAlignmentType.Right;
            style.VerticalAlignment = TextAlignmentType.Center;
            InnerSheet.Cells[row, column].SetStyle(style);
        }

        public void SetCurrentInfo(int row, int column, string infoText)
        {
            InnerSheet.Cells.Merge(row, column, 1, ColumnCount);
            this[row, column].Value = infoText;
            var style = new CellsFactory().CreateStyle();
            style.Font.IsBold = true;
            style.HorizontalAlignment = TextAlignmentType.Left;
            style.VerticalAlignment = TextAlignmentType.Center;
            InnerSheet.Cells[row, column].SetStyle(style);
        }

        public void SetCurrentInfo(int row, int column, int columnCount, string infoText)
        {
            InnerSheet.Cells.Merge(row, column, 1, columnCount);
            this[row, column].Value = infoText;
            var style = new CellsFactory().CreateStyle();
            style.Font.IsBold = true;
            style.HorizontalAlignment = TextAlignmentType.Left;
            style.VerticalAlignment = TextAlignmentType.Center;
            InnerSheet.Cells[row, column].SetStyle(style);
        }

        public void SetHorizontalAlignment(int row, int column, string type)
        {
            var cell = InnerSheet.Cells[row, column];
            var style = cell.GetStyle();
            if (style == null)
                style = new CellsFactory().CreateStyle();
            if (type == "Left")
                style.HorizontalAlignment = TextAlignmentType.Left;
            else if (type == "Center")
                style.HorizontalAlignment = TextAlignmentType.Center;
            else if (type == "Right")
                style.HorizontalAlignment = TextAlignmentType.Right;
            cell.SetStyle(style);
        }

        public void SetVerticalAlignment(int row, string type)
        {
            for (int i = 0; i < ColumnCount; i++)
            {
                SetVerticalAlignment(row, i, type);
            }
        }

        public void SetVerticalAlignment(int row, int column, string type)
        {
            var cell = InnerSheet.Cells[row, column];
            var style = cell.GetStyle();
            if (style == null)
                style = new CellsFactory().CreateStyle();
            if (type == "Top")
                style.VerticalAlignment = TextAlignmentType.Top;
            else if (type == "Center")
                style.VerticalAlignment = TextAlignmentType.Center;
            else if (type == "Bottom")
                style.VerticalAlignment = TextAlignmentType.Bottom;
            cell.SetStyle(style);
        }

        public void Copy(Sheet sourceSheet)
        {
            InnerSheet.Copy(sourceSheet.InnerSheet);
        }

        public void Copy(string sourceSheet)
        {
            var sheet = InnerSheet.Workbook.Worksheets[sourceSheet];
            InnerSheet.Copy(sheet);
        }

        public void CopyRows(int sourceFirstRow, int targetFirstRow, int number)
        {
            InnerSheet.Cells.CopyRows(InnerSheet.Cells, sourceFirstRow, targetFirstRow, number);
        }

        public void InsertRows(int rowIndex, int totalRows)
        {
            InnerSheet.Cells.InsertRows(rowIndex, totalRows);
        }

        public void CopyRange(int sourceFirstRow, int targetFirstRow, int number)
        {
            var sourceRows = InnerSheet.Cells.CreateRange(sourceFirstRow, number, false);
            var targetRows = InnerSheet.Cells.CreateRange(targetFirstRow, number, false);
            targetRows.Copy(sourceRows);
            for (int i = 0; i < number; i++)
            {
                SetRowHeight(targetFirstRow + i, GetRowHeight(i));
            }
        }

        public void AutoFitColumns()
        {
            InnerSheet.AutoFitColumns();
        }

        public void AutoFitRows()
        {
            InnerSheet.AutoFitRows();
        }

        public void ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, bool insertRows)
        {
            InnerSheet.Cells.ImportDataTable(dataTable, isFieldNameShown, firstRow, firstColumn, insertRows);
        }

        public void ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, string dateFormat = null)
        {
            columns = dataTable.Columns;
            var totalRows = dataTable.Rows.Count;
            var totalColumns = dataTable.Columns.Count;
            InnerSheet.Cells.ImportDataTable(dataTable, isFieldNameShown, firstRow, firstColumn, totalRows, totalColumns, true, dateFormat ?? "yyyy-MM-dd HH:mm:ss", false);
            if (isFieldNameShown)
            {
                var style = new CellsFactory().CreateStyle();
                style.Font.IsBold = true;
                for (int i = 0; i < ColumnCount; i++)
                {
                    InnerSheet.Cells[firstRow, i].SetStyle(style);
                }
            }
        }

        public DataTable ExportData(int firstRow = 0, bool asString = true)
        {
            return ExportData(firstRow, 0, asString);
        }

        public DataTable ExportData(int firstRow, int firstColumn, bool asString = true)
        {
            DataTable dt = asString
                ? InnerSheet.Cells.ExportDataTableAsString(firstRow, firstColumn, RowCount - firstRow, ColumnCount - firstColumn, true)
                : InnerSheet.Cells.ExportDataTable(firstRow, firstColumn, RowCount - firstRow, ColumnCount - firstColumn, true);

            if (dt != null)
                dt.TableName = Name;

            return dt;
        }

        public void SetCellValue(string cellName, object value)
        {
            this[cellName].Value = value;
        }

        public void SetCellValue(string cellName, object value, bool hasBorder)
        {
            this[cellName].Value = value;
            if (hasBorder)
            {
                SetCellBorder(cellName, true, true, true, true);
            }
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
            this.InnerSheet.Cells[cellName].SetStyle(style);
        }

        public void SetCellFormatValue(string cellName, string format, params object[] args)
        {
            this[cellName].Value = string.Format(format, args);
        }

        public void SetCellValue(int row, int column, object value)
        {
            this[row, column].Value = value;
        }

        public void SetCellValue(int row, int column, object value, bool isBold = false)
        {
            this[row, column].Value = value;
            if (isBold)
            {
                var style = new CellsFactory().CreateStyle();
                style.Font.IsBold = true;
                InnerSheet.Cells[row, column].SetStyle(style);
            }
        }

        public void SetCellImage(int row, int column, Bitmap bitmap)
        {
            var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
            var index = InnerSheet.Pictures.Add(row, column, stream);
            InnerSheet.Pictures[index].Left = 10;
            InnerSheet.Pictures[index].Top = 10;
            stream.Close();
        }

        public void SetRowValue(int row, int startColumn, params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                this[row, i + startColumn].Value = args[i];
            }
        }

        public double GetRowHeight(int row)
        {
            return InnerSheet.Cells.GetRowHeight(row);
        }

        public void SetRowHeight(int row, double height)
        {
            InnerSheet.Cells.SetRowHeight(row, height);
        }

        public void SetColumnWidth(int column, double width)
        {
            InnerSheet.Cells.SetColumnWidth(column, width);
        }

        public void DeleteRow(int rowIndex)
        {
            InnerSheet.Cells.DeleteRow(rowIndex);
        }

        public void DeleteRows(int rowIndex, int totalRows)
        {
            InnerSheet.Cells.DeleteRows(rowIndex, totalRows);
        }

        public void DeleteColumn(string columnName)
        {
            var columnIndex = -1;
            for (var i = 0; i < columns.Count; i++)
            {
                if (columns[i].ColumnName == columnName)
                {
                    columnIndex = i;
                    break;
                }
            }

            InnerSheet.Cells.DeleteColumn(columnIndex);
            columns.RemoveAt(columnIndex);
        }

        public SheetCell this[string cellName]
        {
            get
            {
                var cell = InnerSheet.Cells[cellName];
                return new SheetCell(cell);
            }
        }

        public SheetCell this[int row, int column]
        {
            get
            {
                var cell = InnerSheet.Cells[row, column];
                return new SheetCell(cell);
            }
        }

        public SheetCell this[int row, string columnName]
        {
            get
            {
                if (columns == null)
                    return null;
                var column = columns.IndexOf(columnName);
                var cell = InnerSheet.Cells[row, column];
                return new SheetCell(cell);
            }
        }

        public bool HasColumn(string columnName)
        {
            return columns.IndexOf(columnName) > -1;
        }

        public void AddColumn(int columnIndex)
        {
            InnerSheet.Cells.InsertColumn(columnIndex);
        }

        public void UnMerge(int firstRow, int firstColumn, int totalRows, int totalColumns)
        {
            InnerSheet.Cells.UnMerge(firstRow, firstColumn, totalRows, totalColumns);
        }

        public void Merge(int firstRow, int firstColumn, int totalRows, int totalColumns)
        {
            InnerSheet.Cells.Merge(firstRow, firstColumn, totalRows, totalColumns);
        }

        public void MergeCells(List<int> mergeColumns, List<int> rowSpans, int startRow, int endRow, int startColumn = 0)
        {
            if (mergeColumns.Count == 0 || rowSpans.Count == 0 || startRow < 0 || endRow < 0 || startRow >= endRow)
                return;

            var totalRows = 0;
            rowSpans.ForEach(rs => { totalRows += rs; });

            if (totalRows != endRow - startRow)
                return;

            for (int i = 0, mcCount = mergeColumns.Count; i < mcCount; i++)
            {
                var currentRow = startRow;

                for (int j = 0, rsCount = rowSpans.Count; j < rsCount; j++)
                {
                    if (rowSpans[j] > 1)
                        InnerSheet.Cells.Merge(currentRow, mergeColumns[i] + startColumn, rowSpans[j], 1);

                    currentRow += rowSpans[j];
                }
            }
        }

        public void SetCellDateFormat(int row, string columnName, string dateFormat)
        {
            if (HasColumn(columnName))
            {
                var cellValue = this[row, columnName].ValueAs<DateTime?>();
                if (cellValue.HasValue)
                    this[row, columnName].Value = cellValue.Value.ToString(dateFormat);
            }
        }

        public void SetCellNumberFormat(int row, string columnName, string numberFormat)
        {
            var format = GetFormatString(numberFormat);
            if (!string.IsNullOrWhiteSpace(format))
            {
                if (HasColumn(columnName))
                {
                    var value = this[row, columnName].ValueAs<string>();
                    decimal num = 0;
                    if (decimal.TryParse(value, out num))
                        this[row, columnName].Value = num.ToString(format);
                }
            }
        }

        private string GetFormatString(string numberFormat)
        {
            if (string.IsNullOrWhiteSpace(numberFormat))
                return string.Empty;

            if (numberFormat.StartsWith("n"))
                numberFormat = numberFormat.Substring(1);

            if (!int.TryParse(numberFormat, out int n))
                return string.Empty;

            if (n <= 0)
                return string.Empty;

            var format = "0.";

            for (var i = 1; i <= n; i++)
                format += "0";

            return format;
        }
    }
}