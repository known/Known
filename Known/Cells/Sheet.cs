using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Known.Cells
{
    public class Sheet
    {
        private DataColumnCollection columns;

        internal Sheet(ISheet sheet)
        {
            InnerSheet = sheet ?? throw new ArgumentNullException(nameof(sheet));
            Index = InnerSheet.Index;
            Name = InnerSheet.Name;
            ColumnCount = InnerSheet.ColumnCount;
            RowCount = InnerSheet.RowCount;
        }

        internal ISheet InnerSheet { get; }
        public int Index { get; }
        public string Name { get; }
        public int ColumnCount { get; }
        public int RowCount { get; }

        public void Copy(Sheet sourceSheet)
        {
            InnerSheet.Copy(sourceSheet.Index);
        }

        public void Copy(string sourceSheetName)
        {
            InnerSheet.Copy(sourceSheetName);
        }

        public void CopyRange(int sourceFirstRow, int targetFirstRow, int number)
        {
            InnerSheet.CopyRange(sourceFirstRow, targetFirstRow, number);
        }

        public void CopyRows(int sourceFirstRow, int targetFirstRow, int number)
        {
            InnerSheet.CopyRows(sourceFirstRow, targetFirstRow, number);
        }

        public void InsertRows(int rowIndex, int totalRows)
        {
            InnerSheet.InsertRows(rowIndex, totalRows);
        }

        public void AutoFitColumns()
        {
            InnerSheet.AutoFitColumns();
        }

        public void AutoFitColumns(int row)
        {
            InnerSheet.AutoFitColumns(row, row);
        }

        public void AutoFitColumns(int firstColumn, int lastColumn)
        {
            InnerSheet.AutoFitColumns(firstColumn, lastColumn);
        }

        public void AutoFitRows()
        {
            InnerSheet.AutoFitRows();
        }

        public void AutoFitRows(int column)
        {
            InnerSheet.AutoFitRows(column, column);
        }

        public void AutoFitRows(int startRow, int endRow)
        {
            InnerSheet.AutoFitRows(startRow, endRow);
        }

        public void SetTextAndMerge(int row, int column, int rowCount, int columnCount, string text, TextAlignment align = TextAlignment.Left)
        {
            InnerSheet.SetTextAndMerge(row, column, rowCount, columnCount, text, align);
        }

        public void SetAlignment(int row, int column, TextAlignment align)
        {
            InnerSheet.SetAlignment(row, column, align);
        }

        public void SetRowAlignment(int row, TextAlignment align)
        {
            for (int i = 0; i < ColumnCount; i++)
            {
                InnerSheet.SetAlignment(row, i, align);
            }
        }

        public void ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, bool insertRows)
        {
            columns = dataTable.Columns;
            InnerSheet.ImportData(dataTable, isFieldNameShown, firstRow, firstColumn, insertRows);
        }

        public void ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            columns = dataTable.Columns;
            var totalRows = dataTable.Rows.Count;
            var totalColumns = dataTable.Columns.Count;
            InnerSheet.ImportData(dataTable, isFieldNameShown, firstRow, firstColumn, totalRows, totalColumns, true, dateFormat, false);
            if (isFieldNameShown)
            {
                for (int i = 0; i < ColumnCount; i++)
                {
                    InnerSheet.SetCellBold(firstRow, i);
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
                ? InnerSheet.ExportDataAsString(firstRow, firstColumn, RowCount - firstRow, ColumnCount - firstColumn, true)
                : InnerSheet.ExportData(firstRow, firstColumn, RowCount - firstRow, ColumnCount - firstColumn, true);

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
                InnerSheet.SetCellBold(row, column);
            }
        }

        public void SetCellBorder(string cellName, bool top, bool right, bool bottom, bool left)
        {
            InnerSheet.SetCellBorder(cellName, top, right, bottom, left);
        }

        public void SetCellImage(int row, int column, Bitmap bitmap)
        {
            var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Bmp);
            InnerSheet.SetCellImage(row, column, stream);
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
            return InnerSheet.GetRowHeight(row);
        }

        public void SetRowHeight(int row, double height)
        {
            InnerSheet.SetRowHeight(row, height);
        }

        public void SetColumnWidth(int column, double width)
        {
            InnerSheet.SetColumnWidth(column, width);
        }

        public void DeleteRow(int rowIndex)
        {
            InnerSheet.DeleteRow(rowIndex);
        }

        public void DeleteRows(int rowIndex, int totalRows)
        {
            InnerSheet.DeleteRows(rowIndex, totalRows);
        }

        public void DeleteColumn(string columnName)
        {
            if (columns == null)
                return;

            var columnIndex = -1;
            for (var i = 0; i < columns.Count; i++)
            {
                if (columns[i].ColumnName == columnName)
                {
                    columnIndex = i;
                    break;
                }
            }

            InnerSheet.DeleteColumn(columnIndex);
            columns.RemoveAt(columnIndex);
        }

        public SheetCell this[string cellName]
        {
            get
            {
                var cell = InnerSheet.GetCell(cellName);
                return new SheetCell(cell);
            }
        }

        public SheetCell this[int row, int column]
        {
            get
            {
                var cell = InnerSheet.GetCell(row, column);
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
                var cell = InnerSheet.GetCell(row, column);
                return new SheetCell(cell);
            }
        }

        public bool HasColumn(string columnName)
        {
            return columns != null && columns.IndexOf(columnName) > -1;
        }

        public void AddColumn(int columnIndex)
        {
            InnerSheet.InsertColumn(columnIndex);
        }

        public void Merge(int firstRow, int firstColumn, int totalRows, int totalColumns)
        {
            InnerSheet.Merge(firstRow, firstColumn, totalRows, totalColumns);
        }

        public void UnMerge(int firstRow, int firstColumn, int totalRows, int totalColumns)
        {
            InnerSheet.UnMerge(firstRow, firstColumn, totalRows, totalColumns);
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
                    {
                        InnerSheet.Merge(currentRow, mergeColumns[i] + startColumn, rowSpans[j], 1);
                    }
                    currentRow += rowSpans[j];
                }
            }
        }

        public void SetCellFormatDate(int row, string columnName, string format)
        {
            if (!HasColumn(columnName))
                return;

            var cellValue = this[row, columnName].ValueAs<DateTime?>();
            if (!cellValue.HasValue)
                return;

            this[row, columnName].Value = cellValue.Value.ToString(format);
        }

        public void SetCellFormatNumber(int row, string columnName, string format)
        {
            if (!HasColumn(columnName))
                return;

            var numberFormat = GetNumberFormatString(format);
            if (string.IsNullOrWhiteSpace(numberFormat))
                return;

            var value = this[row, columnName].ValueAs<string>();
            if (!decimal.TryParse(value, out decimal num))
                return;

            this[row, columnName].Value = num.ToString(numberFormat);
        }

        private string GetNumberFormatString(string format)
        {
            if (string.IsNullOrWhiteSpace(format))
                return string.Empty;

            if (format.StartsWith("n"))
                format = format.Substring(1);

            if (!int.TryParse(format, out int n))
                return string.Empty;

            if (n <= 0)
                return string.Empty;

            var numberFormat = "0.";
            for (var i = 1; i <= n; i++)
                numberFormat += "0";

            return numberFormat;
        }
    }
}