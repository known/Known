namespace Known.Cells;

/// <summary>
/// Excel操作效用类。
/// </summary>
internal sealed class ExcelHelper
{
    internal static byte[] GetExcelBytes<T>(IEnumerable<T> dataSource, List<ExportColumnInfo> exportColumns, Func<T, ExportColumnInfo, object> onExport = null)
    {
        var excel = ExcelFactory.Create();
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
                sheet.SetCellValue(rowIndex, index++, value, cellStyle);
            }
        }

        var stream = excel.SaveToStream();
        return stream.ToArray();
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