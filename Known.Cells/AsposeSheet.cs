using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using Aspose.Cells;
using Aspose.Cells.Drawing;

namespace Known.Cells;

class AsposeSheet : ISheet
{
    private readonly Worksheet sheet;
    private readonly string dateFormat = "yyyy-MM-dd";

    internal AsposeSheet(Worksheet sheet)
    {
        this.sheet = sheet;
    }

    public int MaxDataRow => sheet.Cells.MaxDataRow;
    public int MaxDataColumn => sheet.Cells.MaxDataColumn;

    #region Column
    public double GetColumnWidth(int columnIndex) => sheet.Cells.GetColumnWidth(columnIndex);
    public void SetColumnWidth(int columnIndex, double width) => sheet.Cells.SetColumnWidth(columnIndex, width);
    #endregion

    #region Row
    public int GetFirstRow(string value)
    {
        for (int i = 0; i <= MaxDataRow; i++)
        {
            for (int j = 0; j <= MaxDataColumn; j++)
            {
                var cell = sheet.Cells[i, j];
                if (cell.StringValue == value)
                    return i;
            }
        }

        return 0;
    }

    public double GetRowHeight(int rowIndex) => sheet.Cells.GetRowHeight(rowIndex);
    public void SetRowHeight(int rowIndex, double height) => sheet.Cells.SetRowHeight(rowIndex, height);

    public void SetRowStyle(int rowIndex, StyleInfo info)
    {
        var range = sheet.Cells.CreateRange(rowIndex, 0, 1, sheet.Cells.MaxDataColumn + 1);
        var style = info.CreateStyle(sheet.Cells[rowIndex, 0]);
        range.ApplyStyle(style, new StyleFlag { All = true });
    }

    public void SetRowValues(int rowIndex, int startColumn, params object[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            sheet.Cells[rowIndex, startColumn + i].PutValue(FormatValue(args[i]));
        }
    }

    public void InsertRows(int rowIndex, int totalRows) => sheet.Cells.InsertRows(rowIndex, totalRows);
    #endregion

    #region Cell
    public void SetCellStyle(int rowIndex, int columnIndex, StyleInfo info)
    {
        var cell = sheet.Cells[rowIndex, columnIndex];
        var style = info.CreateStyle(cell);
        cell.SetStyle(style);
    }

    public void SetCellValue(string cellName, object value, StyleInfo info = null)
    {
        var cell = sheet.Cells[cellName];
        cell.PutValue(FormatValue(value));
        if (info != null)
        {
            var style = info.CreateStyle(cell);
            cell.SetStyle(style);
        }
    }

    public void SetCellValue(int rowIndex, int columnIndex, object value, StyleInfo info = null)
    {
        var cell = sheet.Cells[rowIndex, columnIndex];
        cell.PutValue(FormatValue(value));
        if (info != null)
        {
            var style = info.CreateStyle(cell);
            cell.SetStyle(style);
        }
    }

    public void MergeCells(int firstRow, int firstColumn, int totalRows, int totalColumns) => sheet.Cells.Merge(firstRow, firstColumn, totalRows, totalColumns);
    #endregion

    #region Import
    public void ImportData(DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn)
    {
        if (dataTable == null || dataTable.Rows.Count == 0)
            return;

        var totalRows = dataTable.Rows.Count;
        var totalColumns = dataTable.Columns.Count;
        sheet.Cells.ImportDataTable(dataTable, isFieldNameShown, firstRow, firstColumn, totalRows, totalColumns, true, dateFormat, false);
    }

    public void ImportDataByExport(DataTable dataTable)
    {
        if (dataTable == null || dataTable.Rows.Count == 0)
            return;

        var totalRows = dataTable.Rows.Count;
        var totalColumns = dataTable.Columns.Count;
        sheet.Cells.ImportDataTable(dataTable, true, 0, 0, totalRows, totalColumns, true, dateFormat, false);

        var range = sheet.Cells.CreateRange(0, 0, totalRows + 1, totalColumns);
        var info = new StyleInfo { IsBorder = true };
        var style = info.CreateStyle(sheet.Cells[0, 0]);
        range.ApplyStyle(style, new StyleFlag { All = true });

        SetRowStyle(0, new StyleInfo { IsBold = true, FontColor = Color.White, BackgroundColor = Utils.FromHtml("#6D87C1") });
        //sheet.AutoFitColumns(0, 0, 0, totalColumns);
    }
    #endregion

    #region Image
    public void AddPicture(int upperLeftRow, int upperLeftColumn, int left, int top, int width, int height, string path)
    {
        if (!File.Exists(path))
            return;

        var bytes = File.ReadAllBytes(path);
        AddPicture(upperLeftRow, upperLeftColumn, left, top, width, height, bytes);
    }

    public void AddPicture(int upperLeftRow, int upperLeftColumn, int left, int top, int width, int height, byte[] bytes)
    {
        var stream = new MemoryStream(bytes);
        int iIndex = sheet.Pictures.Add(upperLeftRow, upperLeftColumn, stream);
        var picture = sheet.Pictures[iIndex];
        picture.Left = left;
        picture.Top = top;
        picture.Width = width;
        picture.Height = height;
        picture.Placement = PlacementType.Move;
    }
    #endregion

    #region Replace
    public void Replace(string key, string value) => sheet.Replace(key, value);

    public void Replace(int row, int column, string key, object value)
    {
        var cell = sheet.Cells[row, column];
        if (cell.StringValue == key)
        {
            cell.PutValue(value ?? "");
        }
    }

    public void ClearEmpty(string match = "{.?}|{.+}")
    {
        for (int i = 0; i <= MaxDataRow; i++)
        {
            for (int j = 0; j <= MaxDataColumn; j++)
            {
                var cell = sheet.Cells[i, j];
                var value = Regex.Replace(cell.StringValue, match, "");
                cell.PutValue(value);
            }
        }
    }
    #endregion

    #region Private
    private string FormatValue(object value)
    {
        if (value == null)
            return string.Empty;

        var type = value.GetType();
        if (type == typeof(string))
            return $"{value}";

        if (type.Name.Contains("DateTime"))
            return Utils.ConvertTo<DateTime>(value).ToString(dateFormat);

        return $"{value}";
    }
    #endregion
}

static class StyleExtension
{
    internal static Style CreateStyle(this StyleInfo info, Cell cell)
    {
        var style = cell.GetStyle();
        style.IsTextWrapped = info.IsTextWrapped;
        //Font
        style.Font.IsBold = info.IsBold;
        if (info.FontSize.HasValue)
            style.Font.Size = info.FontSize.Value;
        //Color
        if (info.FontColor.HasValue)
            style.Font.Color = info.FontColor.Value;
        if (info.BackgroundColor.HasValue)
        {
            style.Pattern = BackgroundType.Solid;
            style.ForegroundColor = info.BackgroundColor.Value;
        }
        //Alignment
        //if (info.Alignment != null)
        //    style.HorizontalAlignment = (TextAlignmentType)info.Alignment;
        //Border
        if (info.IsBorder)
        {
            var boderStyle = CellBorderType.Thin;
            style.Borders[BorderType.TopBorder].LineStyle = boderStyle;
            style.Borders[BorderType.RightBorder].LineStyle = boderStyle;
            style.Borders[BorderType.BottomBorder].LineStyle = boderStyle;
            style.Borders[BorderType.LeftBorder].LineStyle = boderStyle;
        }
        return style;
    }
}