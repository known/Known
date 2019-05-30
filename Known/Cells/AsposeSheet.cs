using System.Data;
using System.Drawing;
using System.IO;
using Aspose.Cells;

namespace Known.Cells
{
    /// <summary>
    /// Aspose 组件实现的 Excel Sheet 页操作类。
    /// </summary>
    public class AsposeSheet : ISheet
    {
        private readonly Worksheet sheet;

        /// <summary>
        /// 创建一个 Aspose 组件实现的 Excel Sheet 页操作类实例。
        /// </summary>
        /// <param name="wb">Excel 工作簿对象。</param>
        /// <param name="name">Sheet 页名称。</param>
        public AsposeSheet(Workbook wb, string name)
        {
            sheet = wb.Worksheets[name];
            Index = sheet.Index;
            Name = sheet.Name;
            ColumnCount = sheet.Cells.MaxDataColumn + 1;
            RowCount = sheet.Cells.MaxDataRow + 1;
        }

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
        /// 根据名称获取单元格对象。
        /// </summary>
        /// <param name="cellName">单元格名称。</param>
        /// <returns>单元格对象。</returns>
        public ISheetCell GetCell(string cellName)
        {
            var cell = sheet.Cells[cellName];
            return new AsposeSheetCell(cell);
        }

        /// <summary>
        /// 根据行列序号获取单元格对象。
        /// </summary>
        /// <param name="row">行号。</param>
        /// <param name="column">列号。</param>
        /// <returns>单元格对象。</returns>
        public ISheetCell GetCell(int row, int column)
        {
            var cell = sheet.Cells[row, column];
            return new AsposeSheetCell(cell);
        }

        /// <summary>
        /// 复制 Excel 中的指定序号的 Sheet 页。
        /// </summary>
        /// <param name="sourceIndex">源 Sheet 页序号。</param>
        public void Copy(int sourceIndex)
        {
            var sourceSheet = sheet.Workbook.Worksheets[sourceIndex];
            sheet.Copy(sourceSheet);
        }

        /// <summary>
        /// 复制 Excel 中的指定名称的 Sheet 页。
        /// </summary>
        /// <param name="sourceName">源 Sheet 页名称。</param>
        public void Copy(string sourceName)
        {
            var sourceSheet = sheet.Workbook.Worksheets[sourceName];
            sheet.Copy(sourceSheet);
        }

        /// <summary>
        /// 复制指定范围的单元格。
        /// </summary>
        /// <param name="sourceFirstIndex">源首行/列号。</param>
        /// <param name="targetFirstIndex">目的首行/列号。</param>
        /// <param name="number">行/列数。</param>
        public void CopyRange(int sourceFirstIndex, int targetFirstIndex, int number)
        {
            var sourceRows = sheet.Cells.CreateRange(sourceFirstIndex, number, false);
            var targetRows = sheet.Cells.CreateRange(targetFirstIndex, number, false);
            targetRows.Copy(sourceRows);
            for (int i = 0; i < number; i++)
            {
                SetRowHeight(targetFirstIndex + i, GetRowHeight(i));
            }
        }

        /// <summary>
        /// 复制指定范围的行。
        /// </summary>
        /// <param name="sourceFirstRow">源首行号。</param>
        /// <param name="targetFirstRow">目的首行号。</param>
        /// <param name="number">行数。</param>
        public void CopyRows(int sourceFirstRow, int targetFirstRow, int number)
        {
            sheet.Cells.CopyRows(sheet.Cells, sourceFirstRow, targetFirstRow, number);
        }

        /// <summary>
        /// 在指定列序号位置插入一个新列。
        /// </summary>
        /// <param name="columnIndex">列序号。</param>
        public void InsertColumn(int columnIndex)
        {
            sheet.Cells.InsertColumn(columnIndex);
        }

        /// <summary>
        /// 在指定列序号位置插入多个新列。
        /// </summary>
        /// <param name="columnIndex">列序号。</param>
        /// <param name="totalColumns">插入的列数。</param>
        public void InsertColumns(int columnIndex, int totalColumns)
        {
            sheet.Cells.InsertColumns(columnIndex, totalColumns);
        }

        /// <summary>
        /// 在指定行序号位置插入一个新行。
        /// </summary>
        /// <param name="rowIndex">行序号。</param>
        public void InsertRow(int rowIndex)
        {
            sheet.Cells.InsertRow(rowIndex);
        }

        /// <summary>
        /// 在指定行序号位置插入多个新行。
        /// </summary>
        /// <param name="rowIndex">行序号。</param>
        /// <param name="totalRows">插入的行数。</param>
        public void InsertRows(int rowIndex, int totalRows)
        {
            sheet.Cells.InsertRows(rowIndex, totalRows);
        }

        /// <summary>
        /// 删除指定序号的列。
        /// </summary>
        /// <param name="columnIndex">列序号。</param>
        public void DeleteColumn(int columnIndex)
        {
            sheet.Cells.DeleteColumn(columnIndex);
        }

        /// <summary>
        /// 删除指定序号的行。
        /// </summary>
        /// <param name="rowIndex">行序号。</param>
        public void DeleteRow(int rowIndex)
        {
            sheet.Cells.DeleteRow(rowIndex);
        }

        /// <summary>
        /// 删除指定序号位置的多行。
        /// </summary>
        /// <param name="rowIndex">行序号。</param>
        /// <param name="totalRows">删除的行数。</param>
        public void DeleteRows(int rowIndex, int totalRows)
        {
            sheet.Cells.DeleteRows(rowIndex, totalRows);
        }

        /// <summary>
        /// 隐藏指定序号的列。
        /// </summary>
        /// <param name="column">列序号。</param>
        public void HideColumn(int column)
        {
            sheet.Cells.HideColumn(column);
        }

        /// <summary>
        /// 隐藏指定序号位置的多列。
        /// </summary>
        /// <param name="column">列序号。</param>
        /// <param name="totalColumns">隐藏的列数。</param>
        public void HideColumns(int column, int totalColumns)
        {
            sheet.Cells.HideColumns(column, totalColumns);
        }

        /// <summary>
        /// 隐藏指定序号的行。
        /// </summary>
        /// <param name="row">行序号。</param>
        public void HideRow(int row)
        {
            sheet.Cells.HideRow(row);
        }

        /// <summary>
        /// 隐藏指定序号位置的多行。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="totalRows">隐藏的行数。</param>
        public void HideRows(int row, int totalRows)
        {
            sheet.Cells.HideRows(row, totalRows);
        }

        /// <summary>
        /// 自动调整指定列的宽度。
        /// </summary>
        /// <param name="column">列序号。</param>
        public void AutoFitColumn(int column)
        {
            sheet.AutoFitColumn(column);
        }

        /// <summary>
        /// 自动调整所有数据列的宽度。
        /// </summary>
        public void AutoFitColumns()
        {
            sheet.AutoFitColumns(new AutoFitterOptions
            {
                AutoFitMergedCells = true
            });
        }

        /// <summary>
        /// 自动调整指定范围列的宽度。
        /// </summary>
        /// <param name="firstColumn">首列序号。</param>
        /// <param name="lastColumn">尾列序号。</param>
        public void AutoFitColumns(int firstColumn, int lastColumn)
        {
            sheet.AutoFitColumns(firstColumn, lastColumn, new AutoFitterOptions
            {
                AutoFitMergedCells = true
            });
        }

        /// <summary>
        /// 自动调整指定行的高度。
        /// </summary>
        /// <param name="row">行序号。</param>
        public void AutoFitRow(int row)
        {
            sheet.AutoFitRow(row, 0, ColumnCount);
        }

        /// <summary>
        /// 自动调整所有数据行的高度。
        /// </summary>
        public void AutoFitRows()
        {
            sheet.AutoFitRows(new AutoFitterOptions
            {
                AutoFitMergedCells = true
            });
        }

        /// <summary>
        /// 自动调整指定范围行的高度。
        /// </summary>
        /// <param name="startRow">开始行序号。</param>
        /// <param name="endRow">结束行序号。</param>
        public void AutoFitRows(int startRow, int endRow)
        {
            sheet.AutoFitRows(startRow, endRow, new AutoFitterOptions
            {
                AutoFitMergedCells = true
            });
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
            sheet.Cells.Merge(firstRow, firstColumn, totalRows, totalColumns);
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
            sheet.Cells.UnMerge(firstRow, firstColumn, totalRows, totalColumns);
        }

        /// <summary>
        /// 设置单元格内容对齐方式。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="column">列序号。</param>
        /// <param name="align">对齐方式。</param>
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

        /// <summary>
        /// 设置指定单元格的图片。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="column">列序号。</param>
        /// <param name="stream">图片内容流。</param>
        /// <param name="setting">图片位置及大小设置。</param>
        public void SetCellImage(int row, int column, Stream stream, ImageSetting setting = null)
        {
            var index = sheet.Pictures.Add(row, column, stream);
            if (setting != null)
            {
                sheet.Pictures[index].Left = setting.Left;
                sheet.Pictures[index].Top = setting.Top;
                sheet.Pictures[index].Width = setting.Width;
                sheet.Pictures[index].Height = setting.Height;
            }
        }

        /// <summary>
        /// 设置指定单元格的图片。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="column">列序号。</param>
        /// <param name="fileName">图片文件路径。</param>
        /// <param name="setting">图片位置及大小设置。</param>
        public void SetCellImage(int row, int column, string fileName, ImageSetting setting = null)
        {
            var index = sheet.Pictures.Add(row, column, fileName);
            if (setting != null)
            {
                sheet.Pictures[index].Left = setting.Left;
                sheet.Pictures[index].Top = setting.Top;
                sheet.Pictures[index].Width = setting.Width;
                sheet.Pictures[index].Height = setting.Height;
            }
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

        /// <summary>
        /// 设置指定单元格为粗体。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="column">列序号。</param>
        public void SetCellBold(int row, int column)
        {
            var style = new CellsFactory().CreateStyle();
            style.Font.IsBold = true;
            sheet.Cells[row, column].SetStyle(style);
        }

        /// <summary>
        /// 设置指定列表的宽度。
        /// </summary>
        /// <param name="column">列序号。</param>
        /// <param name="width">宽度。</param>
        public void SetColumnWidth(int column, double width)
        {
            sheet.Cells.SetColumnWidth(column, width);
        }

        /// <summary>
        /// 设置指定行的高度。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <param name="height">高度。</param>
        public void SetRowHeight(int row, double height)
        {
            sheet.Cells.SetRowHeight(row, height);
        }

        /// <summary>
        /// 获取指定列的宽度。
        /// </summary>
        /// <param name="column">列序号。</param>
        /// <returns>列宽度。</returns>
        public double GetColumnWidth(int column)
        {
            return sheet.Cells.GetColumnWidth(column);
        }

        /// <summary>
        /// 获取指定行的高度。
        /// </summary>
        /// <param name="row">行序号。</param>
        /// <returns>行高度。</returns>
        public double GetRowHeight(int row)
        {
            return sheet.Cells.GetRowHeight(row);
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
        public int ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, bool insertRows)
        {
            return sheet.Cells.ImportData(dataTable, firstRow, firstColumn, new ImportTableOptions
            {
                IsFieldNameShown = isFieldNameShown,
                InsertRows = insertRows
            });
        }

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
        public int ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, int rowNumber, int columnNumber, bool insertRows, string dateFormatString, bool convertStringToNumber)
        {
            return sheet.Cells.ImportData(dataTable, firstRow, firstColumn, new ImportTableOptions
            {
                IsFieldNameShown = isFieldNameShown,
                InsertRows = insertRows,
                TotalRows = rowNumber,
                TotalColumns = columnNumber,
                DateFormat = dateFormatString,
                ConvertNumericData = convertStringToNumber
            });
        }

        /// <summary>
        /// 导出指定位置的 Sheet 页数据。
        /// </summary>
        /// <param name="firstRow">导出首行序号。</param>
        /// <param name="firstColumn">导出首列序号。</param>
        /// <param name="totalRows">总行数。</param>
        /// <param name="totalColumns">总列数。</param>
        /// <param name="exportColumnName">是否导出列名。</param>
        /// <returns>数据表。</returns>
        public DataTable ExportData(int firstRow, int firstColumn, int totalRows, int totalColumns, bool exportColumnName)
        {
            return sheet.Cells.ExportDataTableAsString(firstRow, firstColumn, totalRows, totalColumns, exportColumnName);
        }

        /// <summary>
        /// 以字符格式导出指定位置的 Sheet 页数据。
        /// </summary>
        /// <param name="firstRow">导出首行序号。</param>
        /// <param name="firstColumn">导出首列序号。</param>
        /// <param name="totalRows">总行数。</param>
        /// <param name="totalColumns">总列数。</param>
        /// <param name="exportColumnName">是否导出列名。</param>
        /// <returns>数据表。</returns>
        public DataTable ExportDataAsString(int firstRow, int firstColumn, int totalRows, int totalColumns, bool exportColumnName)
        {
            return sheet.Cells.ExportDataTableAsString(firstRow, firstColumn, totalRows, totalColumns, exportColumnName);
        }
    }
}
