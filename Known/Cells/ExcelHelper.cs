namespace Known.Cells;

class ExcelDataInfo
{
    public bool IsCsv { get; set; }
    public byte[] Bytes { get; set; }
}

sealed class ExcelHelper
{
    internal static ExcelDataInfo GetExcelBytes<T>(IEnumerable<T> dataSource, List<ExportColumnInfo> exportColumns, Func<T, ExportColumnInfo, object> onExport = null)
    {
        var excel = ExcelFactory.Create();
        if (excel == null)
        {
            var bytes = GetCsvBytes(dataSource, exportColumns, onExport);
            return new ExcelDataInfo { IsCsv = true, Bytes = bytes };
        }

        var sheet = excel.CreateSheet("Sheet1");
        var index = 0;
        var headStyle = new StyleInfo { IsBorder = true, IsBold = true, FontColor = Color.White, BackgroundColor = Utils.FromHtml("#6D87C1") };
        foreach (var item in exportColumns)
        {
            sheet.SetCellValue(0, index++, item.Name, headStyle);
        }

        var rowIndex = 0;
        var isDictionary = typeof(T).IsDictionary();
        foreach (var data in dataSource)
        {
            rowIndex++;
            index = 0;
            foreach (var item in exportColumns)
            {
                var cellStyle = new StyleInfo { IsBorder = true };
                var value = GetCellValue(onExport, isDictionary, data, item, cellStyle);
                sheet.SetCellValue(rowIndex, index++, value, cellStyle);
            }
        }

        var stream = excel.SaveToStream();
        return new ExcelDataInfo { Bytes = stream.ToArray() };
    }

    private static byte[] GetCsvBytes<T>(IEnumerable<T> dataSource, List<ExportColumnInfo> exportColumns, Func<T, ExportColumnInfo, object> onExport)
    {
        var isDictionary = typeof(T).IsDictionary();
        var helper = new CsvHelper();
        helper.AddHeader([.. exportColumns.Select(d => d.Name)]);
        foreach (var data in dataSource)
        {
            var row = new List<string>();
            foreach (var item in exportColumns)
            {
                var cellStyle = new StyleInfo();
                var value = GetCellValue(onExport, isDictionary, data, item, cellStyle);
                row.Add(value?.ToString() ?? string.Empty);
            }
            helper.AddRow([.. row]);
        }

        return helper.ToBytes();
    }

    private static object GetCellValue<T>(Func<T, ExportColumnInfo, object> onExport, bool isDictionary, T data, ExportColumnInfo item, StyleInfo cellStyle)
    {
        object value;
        if (item.IsAdditional)
        {
            value = onExport?.Invoke(data, item);
        }
        else
        {
            value = isDictionary
                  ? (data as Dictionary<string, object>).GetValue(item.Id)
                  : TypeHelper.GetPropertyValue(data, item.Id);
            if (item.Type == FieldType.Switch || item.Type == FieldType.CheckBox)
                value = Utils.ConvertTo<bool>(value) ? "是" : "否";
            else if (item.Type == FieldType.File)
                value = !string.IsNullOrWhiteSpace(value?.ToString()) ? "有" : "无";
            else if (item.Type == FieldType.Date)
            {
                value = Utils.ConvertTo<DateTime?>(value)?.Date;
                cellStyle.Custom = Config.DateFormat;
            }
            else if (item.Type == FieldType.DateTime)
            {
                value = Utils.ConvertTo<DateTime?>(value);
                cellStyle.Custom = Config.DateTimeFormat;
            }
            else if (item.Type == FieldType.Integer)
                value = GetIntegerValue(value);
            else if (item.Type == FieldType.Number)
                value = GetNumberValue(value);
            else if (!string.IsNullOrWhiteSpace(item.Category))
                value = Cache.GetCodeName(item.Category, value?.ToString());
            if (value != null && !string.IsNullOrWhiteSpace(item.Unit))
                value = $"{value} {item.Unit}";
        }

        return value;
    }

    private static object GetIntegerValue(object value)
    {
        if (int.TryParse(value?.ToString(), out var number))
            return number;

        return value;
    }

    private static object GetNumberValue(object value)
    {
        if (decimal.TryParse(value?.ToString(), out var number))
            return number;

        return value;
    }
}