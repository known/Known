using System.Data;
using System.Drawing;
using System.IO;
using Aspose.Cells;

namespace Known.Files
{
    /// <summary>
    /// Aspose组件实现的Sheet类。
    /// </summary>
    public class AsposeSheet : ISheet
    {
        private Worksheet sheet;

        /// <summary>
        /// 构造函数，创建一个Aspose组件实现的Sheet实例。
        /// </summary>
        /// <param name="wb">Excel工作簿。</param>
        /// <param name="name">Sheet名称。</param>
        public AsposeSheet(Workbook wb, string name)
        {
            sheet = wb.Worksheets[name];
            Index = sheet.Index;
            Name = sheet.Name;
            ColumnCount = sheet.Cells.MaxDataColumn + 1;
            RowCount = sheet.Cells.MaxDataRow + 1;
        }

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
        /// 根据Sheet单元格名称获取Sheet单元格对象。
        /// </summary>
        /// <param name="cellName">单元格名称。</param>
        /// <returns>Sheet单元格对象。</returns>
        public ISheetCell GetCell(string cellName)
        {
            var cell = sheet.Cells[cellName];
            return new AsposeSheetCell(cell);
        }

        /// <summary>
        /// 根据Sheet单元格行列号获取Sheet单元格对象。
        /// </summary>
        /// <param name="row">单元格行号。</param>
        /// <param name="column">单元格列号。</param>
        /// <returns>Sheet单元格对象。</returns>
        public ISheetCell GetCell(int row, int column)
        {
            var cell = sheet.Cells[row, column];
            return new AsposeSheetCell(cell);
        }

        /// <summary>
        /// 复制整个Sheet。
        /// </summary>
        /// <param name="sourceSheetIndex">源Sheet索引位置。</param>
        public void Copy(int sourceSheetIndex)
        {
            var sourceSheet = sheet.Workbook.Worksheets[sourceSheetIndex];
            sheet.Copy(sourceSheet);
        }

        /// <summary>
        /// 复制整个Sheet。
        /// </summary>
        /// <param name="sourceSheetName">源Sheet名称。</param>
        public void Copy(string sourceSheetName)
        {
            var sourceSheet = sheet.Workbook.Worksheets[sourceSheetName];
            sheet.Copy(sourceSheet);
        }

        /// <summary>
        /// 复制单元格范围。
        /// </summary>
        /// <param name="sourceFirstRow">原Sheet开始行号。</param>
        /// <param name="targetFirstRow">目标Sheet开始行号。</param>
        /// <param name="number">要复制的行数。</param>
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

        /// <summary>
        /// 复制数据行。
        /// </summary>
        /// <param name="sourceFirstRow">原Sheet开始行号。</param>
        /// <param name="targetFirstRow">目标Sheet开始行号。</param>
        /// <param name="number">要复制的行数。</param>
        public void CopyRows(int sourceFirstRow, int targetFirstRow, int number)
        {
            sheet.Cells.CopyRows(sheet.Cells, sourceFirstRow, targetFirstRow, number);
        }

        /// <summary>
        /// 插入Sheet列。
        /// </summary>
        /// <param name="columnIndex">插入列的索引位置。</param>
        public void InsertColumn(int columnIndex)
        {
            sheet.Cells.InsertColumn(columnIndex);
        }

        /// <summary>
        /// 插入多个Sheet列。
        /// </summary>
        /// <param name="columnIndex">插入列的索引位置。</param>
        /// <param name="totalColumns">插入的列数。</param>
        public void InsertColumns(int columnIndex, int totalColumns)
        {
            sheet.Cells.InsertColumns(columnIndex, totalColumns);
        }

        /// <summary>
        /// 插入Sheet行。
        /// </summary>
        /// <param name="rowIndex">插入行的索引位置。</param>
        public void InsertRow(int rowIndex)
        {
            sheet.Cells.InsertRow(rowIndex);
        }

        /// <summary>
        /// 插入多个Sheet行。
        /// </summary>
        /// <param name="rowIndex">插入行的索引位置。</param>
        /// <param name="totalRows">插入的行数。</param>
        public void InsertRows(int rowIndex, int totalRows)
        {
            sheet.Cells.InsertRows(rowIndex, totalRows);
        }

        /// <summary>
        /// 删除Sheet列。
        /// </summary>
        /// <param name="columnIndex">删除列的索引位置。</param>
        public void DeleteColumn(int columnIndex)
        {
            sheet.Cells.DeleteColumn(columnIndex);
        }

        /// <summary>
        /// 删除Sheet行。
        /// </summary>
        /// <param name="rowIndex">删除行的索引位置。</param>
        public void DeleteRow(int rowIndex)
        {
            sheet.Cells.DeleteRow(rowIndex);
        }

        /// <summary>
        /// 删除多个Sheet行。
        /// </summary>
        /// <param name="rowIndex">删除列的索引位置。</param>
        /// <param name="totalRows">删除的行数。</param>
        public void DeleteRows(int rowIndex, int totalRows)
        {
            sheet.Cells.DeleteRows(rowIndex, totalRows);
        }

        /// <summary>
        /// 隐藏Sheet列。
        /// </summary>
        /// <param name="column">隐藏列的索引位置</param>
        public void HideColumn(int column)
        {
            sheet.Cells.HideColumn(column);
        }

        /// <summary>
        /// 隐藏多个Sheet列。
        /// </summary>
        /// <param name="column">隐藏列的索引位置。</param>
        /// <param name="totalColumns">隐藏的列数。</param>
        public void HideColumns(int column, int totalColumns)
        {
            sheet.Cells.HideColumns(column, totalColumns);
        }

        /// <summary>
        /// 隐藏Sheet行。
        /// </summary>
        /// <param name="row">隐藏行的索引位置。</param>
        public void HideRow(int row)
        {
            sheet.Cells.HideRow(row);
        }

        /// <summary>
        /// 隐藏多个Sheet行。
        /// </summary>
        /// <param name="row">隐藏行的索引位置。</param>
        /// <param name="totalRows">隐藏的行数。</param>
        public void HideRows(int row, int totalRows)
        {
            sheet.Cells.HideRows(row, totalRows);
        }

        /// <summary>
        /// 自动调整所有列的宽度。
        /// </summary>
        public void AutoFitColumns()
        {
            sheet.AutoFitColumns();
        }

        /// <summary>
        /// 自动调整所有行的高度。
        /// </summary>
        public void AutoFitRows()
        {
            sheet.AutoFitRows();
        }

        /// <summary>
        /// 合并单元格。
        /// </summary>
        /// <param name="firstRow">开始合并行号。</param>
        /// <param name="firstColumn">开始合并列号。</param>
        /// <param name="totalRows">合并的总行数。</param>
        /// <param name="totalColumns">合并的总列数。</param>
        public void Merge(int firstRow, int firstColumn, int totalRows, int totalColumns)
        {
            sheet.Cells.Merge(firstRow, firstColumn, totalRows, totalColumns);
        }

        /// <summary>
        /// 取消合并单元格。
        /// </summary>
        /// <param name="firstRow">开始合并行号。</param>
        /// <param name="firstColumn">开始合并列号。</param>
        /// <param name="totalRows">合并的总行数。</param>
        /// <param name="totalColumns">合并的总列数。</param>
        public void UnMerge(int firstRow, int firstColumn, int totalRows, int totalColumns)
        {
            sheet.Cells.UnMerge(firstRow, firstColumn, totalRows, totalColumns);
        }

        /// <summary>
        /// 设置单元格对齐方式。
        /// </summary>
        /// <param name="row">单元格行号。</param>
        /// <param name="column">单元格列号。</param>
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
        /// 设置单元格图片。
        /// </summary>
        /// <param name="row">单元格行号。</param>
        /// <param name="column">单元格列号。</param>
        /// <param name="stream">图片文件流。</param>
        public void SetCellImage(int row, int column, Stream stream)
        {
            var index = sheet.Pictures.Add(row, column, stream);
            sheet.Pictures[index].Left = 10;
            sheet.Pictures[index].Top = 10;
        }

        /// <summary>
        /// 设置单元格图片。
        /// </summary>
        /// <param name="row">单元格行号。</param>
        /// <param name="column">单元格列号。</param>
        /// <param name="fileName">图片文件路径。</param>
        public void SetCellImage(int row, int column, string fileName)
        {
            var index = sheet.Pictures.Add(row, column, fileName);
            sheet.Pictures[index].Left = 10;
            sheet.Pictures[index].Top = 10;
        }

        /// <summary>
        /// 设置单元格边框。
        /// </summary>
        /// <param name="cellName">单元格名。</param>
        /// <param name="top">是否设置上边框。</param>
        /// <param name="right">是否设置右边框。</param>
        /// <param name="bottom">是否设置下边框。</param>
        /// <param name="left">是否设置左边框。</param>
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
        /// 设置单元格粗体。
        /// </summary>
        /// <param name="row">单元格行号。</param>
        /// <param name="column">单元格列号。</param>
        public void SetCellBold(int row, int column)
        {
            var style = new CellsFactory().CreateStyle();
            style.Font.IsBold = true;
            sheet.Cells[row, column].SetStyle(style);
        }

        /// <summary>
        /// 设置Sheet列宽度。
        /// </summary>
        /// <param name="column">列的索引位置。</param>
        /// <param name="width">宽度。</param>
        public void SetColumnWidth(int column, double width)
        {
            sheet.Cells.SetColumnWidth(column, width);
        }

        /// <summary>
        /// 设置Sheet行高度。
        /// </summary>
        /// <param name="row">行的索引位置。</param>
        /// <param name="height">高度。</param>
        public void SetRowHeight(int row, double height)
        {
            sheet.Cells.SetRowHeight(row, height);
        }

        /// <summary>
        /// 获取列的宽度。
        /// </summary>
        /// <param name="column">列的索引位置。</param>
        /// <returns>列的宽度。</returns>
        public double GetColumnWidth(int column)
        {
            return sheet.Cells.GetColumnWidth(column);
        }

        /// <summary>
        /// 获取行的高度。
        /// </summary>
        /// <param name="row">行的索引位置。</param>
        /// <returns>行的高度。</returns>
        public double GetRowHeight(int row)
        {
            return sheet.Cells.GetRowHeight(row);
        }

        /// <summary>
        /// 将DataTable导入到Sheet中。
        /// </summary>
        /// <param name="dataTable">导入的数据。</param>
        /// <param name="isFieldNameShown">是否显示数据列名。</param>
        /// <param name="firstRow">导入的首行位置。</param>
        /// <param name="firstColumn">导入的首列位置。</param>
        /// <param name="insertRows">是否插入行。</param>
        /// <returns>导入的行数。</returns>
        public int ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, bool insertRows)
        {
            return sheet.Cells.ImportDataTable(dataTable, isFieldNameShown, firstRow, firstColumn, insertRows);
        }

        /// <summary>
        /// 将DataTable导入到Sheet中。
        /// </summary>
        /// <param name="dataTable">导入的数据。</param>
        /// <param name="isFieldNameShown">是否显示数据列名。</param>
        /// <param name="firstRow">导入的首行位置。</param>
        /// <param name="firstColumn">导入的首列位置。</param>
        /// <param name="rowNumber">导入的行数。</param>
        /// <param name="columnNumber">导入的列数。</param>
        /// <param name="insertRows">是否插入行。</param>
        /// <param name="dateFormatString">日期类型的格式。</param>
        /// <param name="convertStringToNumber">是否转换字符串为数值。</param>
        /// <returns>导入的行数。</returns>
        public int ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, int rowNumber, int columnNumber, bool insertRows, string dateFormatString, bool convertStringToNumber)
        {
            return sheet.Cells.ImportDataTable(dataTable, isFieldNameShown, firstRow, firstColumn, rowNumber, columnNumber, insertRows, dateFormatString, convertStringToNumber);
        }

        /// <summary>
        /// 导出Sheet中的数据。
        /// </summary>
        /// <param name="firstRow">导出的首行位置。</param>
        /// <param name="firstColumn">导出的首列位置。</param>
        /// <param name="totalRows">导出的行数。</param>
        /// <param name="totalColumns">导出的列数。</param>
        /// <param name="exportColumnName">是否导出列名。</param>
        /// <returns>导出的数据。</returns>
        public DataTable ExportData(int firstRow, int firstColumn, int totalRows, int totalColumns, bool exportColumnName)
        {
            return sheet.Cells.ExportDataTableAsString(firstRow, firstColumn, totalRows, totalColumns, exportColumnName);
        }

        /// <summary>
        /// 以字符串的形式导出Sheet中的数据。
        /// </summary>
        /// <param name="firstRow">导出的首行位置。</param>
        /// <param name="firstColumn">导出的首列位置。</param>
        /// <param name="totalRows">导出的行数。</param>
        /// <param name="totalColumns">导出的列数。</param>
        /// <param name="exportColumnName">是否导出列名。</param>
        /// <returns>导出的数据。</returns>
        public DataTable ExportDataAsString(int firstRow, int firstColumn, int totalRows, int totalColumns, bool exportColumnName)
        {
            return sheet.Cells.ExportDataTableAsString(firstRow, firstColumn, totalRows, totalColumns, exportColumnName);
        }
    }
}
