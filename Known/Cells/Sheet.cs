using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Known.Cells
{
    /// <summary>
    /// Sheet 页操作类。
    /// </summary>
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

        /// <summary>
        /// 取得 Sheet 页的索引序号。
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// 取得 Sheet 页的名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 取得 Sheet 页的数据列数。
        /// </summary>
        public int ColumnCount { get; }

        /// <summary>
        /// 取得 Sheet 页的数据行数。
        /// </summary>
        public int RowCount { get; }

        /// <summary>
        /// 复制 Excel 中的指定名称的 Sheet 页。
        /// </summary>
        /// <param name="sourceSheet">源 Sheet 页对象。</param>
        public void Copy(Sheet sourceSheet)
        {
            InnerSheet.Copy(sourceSheet.Index);
        }

        /// <summary>
        /// 复制 Excel 中的指定名称的 Sheet 页。
        /// </summary>
        /// <param name="sourceName">源 Sheet 页名称。</param>
        public void Copy(string sourceName)
        {
            InnerSheet.Copy(sourceName);
        }

        /// <summary>
        /// 复制指定范围的单元格。
        /// </summary>
        /// <param name="sourceFirstIndex">源首行/列号。</param>
        /// <param name="targetFirstIndex">目的首行/列号。</param>
        /// <param name="number">行/列数。</param>
        public void CopyRange(int sourceFirstIndex, int targetFirstIndex, int number)
        {
            InnerSheet.CopyRange(sourceFirstIndex, targetFirstIndex, number);
        }

        /// <summary>
        /// 复制指定范围的行。
        /// </summary>
        /// <param name="sourceFirstRow">源首行号。</param>
        /// <param name="targetFirstRow">目的首行号。</param>
        /// <param name="number">行数。</param>
        public void CopyRows(int sourceFirstRow, int targetFirstRow, int number)
        {
            InnerSheet.CopyRows(sourceFirstRow, targetFirstRow, number);
        }

        /// <summary>
        /// 在指定行序号位置插入多个新行。
        /// </summary>
        /// <param name="rowIndex">行序号。</param>
        /// <param name="totalRows">插入的行数。</param>
        public void InsertRows(int rowIndex, int totalRows)
        {
            InnerSheet.InsertRows(rowIndex, totalRows);
        }

        /// <summary>
        /// 自动调整指定列的宽度。
        /// </summary>
        /// <param name="column">列序号。</param>
        public void AutoFitColumn(int column)
        {
            InnerSheet.AutoFitColumn(column);
        }

        /// <summary>
        /// 自动调整所有数据列的宽度。
        /// </summary>
        public void AutoFitColumns()
        {
            InnerSheet.AutoFitColumns();
        }

        /// <summary>
        /// 自动调整指定范围列的宽度。
        /// </summary>
        /// <param name="firstColumn">首列序号。</param>
        /// <param name="lastColumn">尾列序号。</param>
        public void AutoFitColumns(int firstColumn, int lastColumn)
        {
            InnerSheet.AutoFitColumns(firstColumn, lastColumn);
        }

        /// <summary>
        /// 自动调整指定行的高度。
        /// </summary>
        /// <param name="row">行序号。</param>
        public void AutoFitRow(int row)
        {
            InnerSheet.AutoFitRow(row);
        }

        /// <summary>
        /// 自动调整所有数据行的高度。
        /// </summary>
        public void AutoFitRows()
        {
            InnerSheet.AutoFitRows();
        }

        /// <summary>
        /// 自动调整指定范围行的高度。
        /// </summary>
        /// <param name="startRow">开始行序号。</param>
        /// <param name="endRow">结束行序号。</param>
        public void AutoFitRows(int startRow, int endRow)
        {
            InnerSheet.AutoFitRows(startRow, endRow);
        }

        /// <summary>
        /// 设置指定单元格文本合并和对齐方式。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="column">列序号。</param>
        /// <param name="rowCount">合并列数。</param>
        /// <param name="columnCount">合并行数。</param>
        /// <param name="text">文本内容。</param>
        /// <param name="align">对齐方式，默认左对齐。</param>
        public void SetTextAndMerge(int row, int column, int rowCount, int columnCount, string text, TextAlignment align = TextAlignment.Left)
        {
            InnerSheet.SetTextAndMerge(row, column, rowCount, columnCount, text, align);
        }

        /// <summary>
        /// 设置指定单元格的内容对齐方式。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="column">列序号。</param>
        /// <param name="align">对齐方式。</param>
        public void SetAlignment(int row, int column, TextAlignment align)
        {
            InnerSheet.SetAlignment(row, column, align);
        }

        /// <summary>
        /// 设置指定行所有单元格的对齐方式。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="align">对齐方式。</param>
        public void SetRowAlignment(int row, TextAlignment align)
        {
            for (int i = 0; i < ColumnCount; i++)
            {
                InnerSheet.SetAlignment(row, i, align);
            }
        }

        /// <summary>
        /// 将数据表导入到 Sheet 页的指定位置。
        /// </summary>
        /// <param name="dataTable">数据表。</param>
        /// <param name="isFieldNameShown">是否显示列名。</param>
        /// <param name="firstRow">导入首行序号。</param>
        /// <param name="firstColumn">导入首列序号。</param>
        /// <param name="insertRows">是否在指定插入行。</param>
        /// <returns>导入的行数。</returns>
        public void ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, bool insertRows)
        {
            columns = dataTable.Columns;
            InnerSheet.ImportData(dataTable, isFieldNameShown, firstRow, firstColumn, insertRows);
        }

        /// <summary>
        /// 将数据表插入到 Sheet 页的指定位置。
        /// </summary>
        /// <param name="dataTable">数据表。</param>
        /// <param name="isFieldNameShown">是否显示列名。</param>
        /// <param name="firstRow">导入首行序号。</param>
        /// <param name="firstColumn">导入首列序号。</param>
        /// <param name="dateFormat">日期格式，默认：yyyy-MM-dd HH:mm:ss。</param>
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

        /// <summary>
        /// 从指定行序号开始导出 Sheet 页中所有数据，默认首列为第一列。
        /// </summary>
        /// <param name="firstRow">导出首行序号，默认：0。</param>
        /// <param name="asString">是否以字符格式导出，默认：True。</param>
        /// <returns>数据表。</returns>
        public DataTable ExportData(int firstRow = 0, bool asString = true)
        {
            return ExportData(firstRow, 0, asString);
        }

        /// <summary>
        /// 从指定行列序号开始导出 Sheet 页中所有数据。
        /// </summary>
        /// <param name="firstRow">导出首行序号。</param>
        /// <param name="firstColumn">导出首列序号。</param>
        /// <param name="asString">是否以字符格式导出，默认：True。</param>
        /// <returns>数据表。</returns>
        public DataTable ExportData(int firstRow, int firstColumn, bool asString = true)
        {
            DataTable dt = asString
                ? InnerSheet.ExportDataAsString(firstRow, firstColumn, RowCount - firstRow, ColumnCount - firstColumn, true)
                : InnerSheet.ExportData(firstRow, firstColumn, RowCount - firstRow, ColumnCount - firstColumn, true);

            if (dt != null)
                dt.TableName = Name;

            return dt;
        }

        /// <summary>
        /// 设置指定名称的单元格数据。
        /// </summary>
        /// <param name="cellName">单元格名称。</param>
        /// <param name="value">数据。</param>
        public void SetCellValue(string cellName, object value)
        {
            this[cellName].Value = value;
        }

        /// <summary>
        /// 设置指定名称的单元格数据，是否带边框格式。
        /// </summary>
        /// <param name="cellName">单元格名称。</param>
        /// <param name="value">数据。</param>
        /// <param name="hasBorder">是否带边框。</param>
        public void SetCellValue(string cellName, object value, bool hasBorder)
        {
            this[cellName].Value = value;
            if (hasBorder)
            {
                SetCellBorder(cellName, true, true, true, true);
            }
        }

        /// <summary>
        /// 设置指定行列序号的单元格数据。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="column">列序号。</param>
        /// <param name="value">数据。</param>
        public void SetCellValue(int row, int column, object value)
        {
            this[row, column].Value = value;
        }

        /// <summary>
        /// 设置指定单元格的边框。
        /// </summary>
        /// <param name="cellName">单元格名。</param>
        /// <param name="top">是否有上边框。</param>
        /// <param name="right">是否有右边框。</param>
        /// <param name="bottom">是否有下边框。</param>
        /// <param name="left">是否有左边框。</param>
        public void SetCellBorder(string cellName, bool top, bool right, bool bottom, bool left)
        {
            InnerSheet.SetCellBorder(cellName, top, right, bottom, left);
        }

        /// <summary>
        /// 设置指定单元格的图片。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="column">列序号。</param>
        /// <param name="bitmap">图片位图对象。</param>
        public void SetCellImage(int row, int column, Bitmap bitmap)
        {
            var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Bmp);
            InnerSheet.SetCellImage(row, column, stream);
            stream.Close();
        }

        /// <summary>
        /// 批量设置指定行多列数据。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="startColumn">开始列序号。</param>
        /// <param name="args">列数据集合。</param>
        public void SetRowValue(int row, int startColumn, params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                this[row, i + startColumn].Value = args[i];
            }
        }

        /// <summary>
        /// 获取指定行的高度。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <returns>行高度。</returns>
        public double GetRowHeight(int row)
        {
            return InnerSheet.GetRowHeight(row);
        }

        /// <summary>
        /// 设置指定行的高度。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="height">高度。</param>
        public void SetRowHeight(int row, double height)
        {
            InnerSheet.SetRowHeight(row, height);
        }

        /// <summary>
        /// 设置指定列表的宽度。
        /// </summary>
        /// <param name="column">列序号。</param>
        /// <param name="width">宽度。</param>
        public void SetColumnWidth(int column, double width)
        {
            InnerSheet.SetColumnWidth(column, width);
        }

        /// <summary>
        /// 删除指定序号的行。
        /// </summary>
        /// <param name="rowIndex">行序号。</param>
        public void DeleteRow(int rowIndex)
        {
            InnerSheet.DeleteRow(rowIndex);
        }

        /// <summary>
        /// 删除指定序号位置的多行。
        /// </summary>
        /// <param name="rowIndex">行序号。</param>
        /// <param name="totalRows">删除的行数。</param>
        public void DeleteRows(int rowIndex, int totalRows)
        {
            InnerSheet.DeleteRows(rowIndex, totalRows);
        }

        /// <summary>
        /// 删除指定名称的列。
        /// </summary>
        /// <param name="columnName">列名称。</param>
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

        /// <summary>
        /// 取得指定名称索引的单元格对象。
        /// </summary>
        /// <param name="cellName">单元格名称。</param>
        /// <returns>单元格对象。</returns>
        public SheetCell this[string cellName]
        {
            get
            {
                var cell = InnerSheet.GetCell(cellName);
                return new SheetCell(cell);
            }
        }

        /// <summary>
        /// 取得指定行列序号索引的单元格对象。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="column">列序号。</param>
        /// <returns>单元格对象。</returns>
        public SheetCell this[int row, int column]
        {
            get
            {
                var cell = InnerSheet.GetCell(row, column);
                return new SheetCell(cell);
            }
        }

        /// <summary>
        /// 取得指定行序号和列名称索引的单元格对象。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="columnName">列名称。</param>
        /// <returns>单元格对象。</returns>
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

        /// <summary>
        /// 判断是否有指定名称的列。
        /// </summary>
        /// <param name="columnName">列名称。</param>
        /// <returns>有返回 True，否则返回 False。</returns>
        public bool HasColumn(string columnName)
        {
            return columns != null && columns.IndexOf(columnName) > -1;
        }

        /// <summary>
        /// 在指定列序号位置添加一个新列。
        /// </summary>
        /// <param name="columnIndex">列序号。</param>
        public void AddColumn(int columnIndex)
        {
            InnerSheet.InsertColumn(columnIndex);
        }

        /// <summary>
        /// 合并指定范围的单元格。
        /// </summary>
        /// <param name="firstRow">首行序号。</param>
        /// <param name="firstColumn">首列序号。</param>
        /// <param name="totalRows">总行数。</param>
        /// <param name="totalColumns">总列数。</param>
        public void Merge(int firstRow, int firstColumn, int totalRows, int totalColumns)
        {
            InnerSheet.Merge(firstRow, firstColumn, totalRows, totalColumns);
        }

        /// <summary>
        /// 不合并指定范围的单元格。
        /// </summary>
        /// <param name="firstRow">首行序号。</param>
        /// <param name="firstColumn">首列序号。</param>
        /// <param name="totalRows">总行数。</param>
        /// <param name="totalColumns">总列数。</param>
        public void UnMerge(int firstRow, int firstColumn, int totalRows, int totalColumns)
        {
            InnerSheet.UnMerge(firstRow, firstColumn, totalRows, totalColumns);
        }

        /// <summary>
        /// 合并指定列所有相同数据的行。
        /// </summary>
        /// <param name="mergeColumns">要合并行的列集合。</param>
        /// <param name="rowSpans">跨行大小集合。</param>
        /// <param name="startRow">开始行序号。</param>
        /// <param name="endRow">结束行序号。</param>
        /// <param name="startColumn">开始列序号，默认：0。</param>
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

        /// <summary>
        /// 设置指定行序号和列名称的日期格式。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="columnName">列名称。</param>
        /// <param name="format">日期格式。</param>
        public void SetCellFormatDate(int row, string columnName, string format)
        {
            if (!HasColumn(columnName))
                return;

            var cellValue = this[row, columnName].ValueAs<DateTime?>();
            if (!cellValue.HasValue)
                return;

            this[row, columnName].Value = cellValue.Value.ToString(format);
        }

        /// <summary>
        /// 设置指定行序号和列名称的数值格式。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="columnName">列名称。</param>
        /// <param name="format">数值格式。</param>
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