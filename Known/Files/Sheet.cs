using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Known.Files
{
    /// <summary>
    /// Sheet类。
    /// </summary>
    public class Sheet
    {
        private DataColumnCollection columns;

        /// <summary>
        /// 构造函数，创建一个Sheet实例。
        /// </summary>
        /// <param name="sheet">Sheet接口。</param>
        internal Sheet(ISheet sheet)
        {
            InnerSheet = sheet ?? throw new ArgumentNullException("sheet");

            Index = InnerSheet.Index;
            Name = InnerSheet.Name;
            ColumnCount = InnerSheet.ColumnCount;
            RowCount = InnerSheet.RowCount;
        }

        /// <summary>
        /// 取得Sheet接口。
        /// </summary>
        internal ISheet InnerSheet { get; }

        /// <summary>
        /// 取得Excel中Sheet的索引位置。
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// 取得Excel中Sheet的名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 取得Excel中Sheet的数据列数。
        /// </summary>
        public int ColumnCount { get; }

        /// <summary>
        /// 取得Excel中Sheet的数据行数。
        /// </summary>
        public int RowCount { get; }

        /// <summary>
        /// 复制整个Sheet。
        /// </summary>
        /// <param name="sourceSheet">源Sheet对象。</param>
        public void Copy(Sheet sourceSheet)
        {
            InnerSheet.Copy(sourceSheet.Index);
        }

        /// <summary>
        /// 复制整个Sheet。
        /// </summary>
        /// <param name="sourceSheetName">源Sheet名称。</param>
        public void Copy(string sourceSheetName)
        {
            InnerSheet.Copy(sourceSheetName);
        }

        /// <summary>
        /// 复制单元格范围。
        /// </summary>
        /// <param name="sourceFirstRow">原Sheet开始行号。</param>
        /// <param name="targetFirstRow">目标Sheet开始行号。</param>
        /// <param name="number">要复制的行数。</param>
        public void CopyRange(int sourceFirstRow, int targetFirstRow, int number)
        {
            InnerSheet.CopyRange(sourceFirstRow, targetFirstRow, number);
        }

        /// <summary>
        /// 复制数据行。
        /// </summary>
        /// <param name="sourceFirstRow">原Sheet开始行号。</param>
        /// <param name="targetFirstRow">目标Sheet开始行号。</param>
        /// <param name="number">要复制的行数。</param>
        public void CopyRows(int sourceFirstRow, int targetFirstRow, int number)
        {
            InnerSheet.CopyRows(sourceFirstRow, targetFirstRow, number);
        }

        /// <summary>
        /// 插入多个Sheet行。
        /// </summary>
        /// <param name="rowIndex">插入行的索引位置。</param>
        /// <param name="totalRows">插入的行数。</param>
        public void InsertRows(int rowIndex, int totalRows)
        {
            InnerSheet.InsertRows(rowIndex, totalRows);
        }

        /// <summary>
        /// 自动调整所有列的宽度。
        /// </summary>
        public void AutoFitColumns()
        {
            InnerSheet.AutoFitColumns();
        }

        /// <summary>
        /// 自动调整所有行的高度。
        /// </summary>
        public void AutoFitRows()
        {
            InnerSheet.AutoFitRows();
        }

        /// <summary>
        /// 设置单元格文本并合并。
        /// </summary>
        /// <param name="row">单元格行号。</param>
        /// <param name="column">单元格列号。</param>
        /// <param name="rowCount">合并的行数。</param>
        /// <param name="columnCount">合并的列数。</param>
        /// <param name="text">设置的文本。</param>
        /// <param name="align">文本的对齐方式。</param>
        public void SetTextAndMerge(int row, int column, int rowCount, int columnCount, string text, TextAlignment align = TextAlignment.Left)
        {
            InnerSheet.SetTextAndMerge(row, column, rowCount, columnCount, text, align);
        }

        /// <summary>
        /// 设置单元格对齐方式。
        /// </summary>
        /// <param name="row">单元格行号。</param>
        /// <param name="column">单元格列号。</param>
        /// <param name="align">对齐方式。</param>
        public void SetAlignment(int row, int column, TextAlignment align)
        {
            InnerSheet.SetAlignment(row, column, align);
        }

        /// <summary>
        /// 设置整个行单元格对齐方式。
        /// </summary>
        /// <param name="row">设置的行号。</param>
        /// <param name="align">对齐方式。</param>
        public void SetRowAlignment(int row, TextAlignment align)
        {
            for (int i = 0; i < ColumnCount; i++)
            {
                InnerSheet.SetAlignment(row, i, align);
            }
        }

        /// <summary>
        /// 将DataTable导入到Sheet中。
        /// </summary>
        /// <param name="dataTable">导入的数据。</param>
        /// <param name="isFieldNameShown">是否显示数据列名。</param>
        /// <param name="firstRow">导入的首行位置。</param>
        /// <param name="firstColumn">导入的首列位置。</param>
        /// <param name="insertRows">是否插入行。</param>
        public void ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, bool insertRows)
        {
            columns = dataTable.Columns;
            InnerSheet.ImportData(dataTable, isFieldNameShown, firstRow, firstColumn, insertRows);
        }

        /// <summary>
        /// 将DataTable导入到Sheet中。
        /// </summary>
        /// <param name="dataTable">导入的数据。</param>
        /// <param name="isFieldNameShown">是否显示数据列名。</param>
        /// <param name="firstRow">导入的首行位置。</param>
        /// <param name="firstColumn">导入的首列位置。</param>
        /// <param name="dateFormat">日期类型的格式。</param>
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

        public void SetCellBorder(string cellName, bool top, bool right, bool bottom, bool left)
        {
            InnerSheet.SetCellBorder(cellName, top, right, bottom, left);
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

        public void UnMerge(int firstRow, int firstColumn, int totalRows, int totalColumns)
        {
            InnerSheet.UnMerge(firstRow, firstColumn, totalRows, totalColumns);
        }

        public void Merge(int firstRow, int firstColumn, int totalRows, int totalColumns)
        {
            InnerSheet.Merge(firstRow, firstColumn, totalRows, totalColumns);
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
                        InnerSheet.Merge(currentRow, mergeColumns[i] + startColumn, rowSpans[j], 1);

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
                    if (decimal.TryParse(value, out decimal num))
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