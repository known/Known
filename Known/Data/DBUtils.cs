namespace Known.Data;

/// <summary>
/// 数据库操作效用类。
/// </summary>
public sealed class DbUtils
{
    private DbUtils() { }

    /// <summary>
    /// 将DataReader转换成泛型对象。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="reader">DataReader。</param>
    /// <returns>泛型对象。</returns>
    public static object ConvertTo<T>(IDataReader reader)
    {
        var dic = GetDictionary(reader);
        var type = typeof(T);
        if (type == typeof(Dictionary<string, object>))
            return dic;

        return ConvertTo<T>(dic);
    }

    /// <summary>
    /// 将DataRow转换成泛型对象。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="row">DataRow。</param>
    /// <returns>泛型对象。</returns>
    public static object ConvertTo<T>(DataRow row)
    {
        var dic = GetDictionary(row);
        var type = typeof(T);
        if (type == typeof(Dictionary<string, object>))
            return dic;

        return ConvertTo<T>(dic);
    }

    /// <summary>
    /// 将匿名参数对象转换成字典对象。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="value">匿名参数对象。</param>
    /// <returns>字典对象。</returns>
    public static Dictionary<string, object> ToDictionary<T>(T value)
    {
        if (value is Dictionary<string, object> dictionary)
            return dictionary;

        if (value != null && TypeHelper.IsAnonymousType(value))
            return Utils.MapTo<Dictionary<string, object>>(value);

        var dic = new Dictionary<string, object>();
        var properties = TypeHelper.Properties(typeof(T));
        foreach (var item in properties)
        {
            if (item.CanRead && !item.GetMethod.IsVirtual)
            {
                dic[item.Name] = value == null ? null : item.GetValue(value, null);
            }
        }
        return dic;
    }

    /// <summary>
    /// 将指定泛型类型的属性转成字典对象，属性值默认为空。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>字典对象。</returns>
    public static Dictionary<string, object> ToDictionary<T>()
    {
        var dic = new Dictionary<string, object>();
        var properties = TypeHelper.Properties(typeof(T));
        foreach (var item in properties)
        {
            if (item.CanRead && item.CanWrite && !item.GetMethod.IsVirtual)
            {
                dic[item.Name] = null;
            }
        }
        return dic;
    }

    internal static Dictionary<string, object> GetDictionary(IDataReader reader)
    {
        var dic = new Dictionary<string, object>();
        for (int i = 0; i < reader.FieldCount; i++)
        {
            var name = reader.GetName(i).Replace("_", "");
            if (name == "rowno") //去掉row_number产生的字段
                continue;

            var value = reader[i];
            dic[name] = value == DBNull.Value ? null : value;
        }
        return dic;
    }

    private static Dictionary<string, object> GetDictionary(DataRow row)
    {
        var dic = new Dictionary<string, object>();
        foreach (DataColumn item in row.Table.Columns)
        {
            var name = item.ColumnName.Replace("_", "");
            if (name == "rowno") //去掉row_number产生的字段
                continue;

            var value = row[item.ColumnName];
            dic[name] = value == DBNull.Value ? null : value;
        }
        return dic;
    }

    private static object ConvertTo<T>(Dictionary<string, object> dic)
    {
        var obj = Activator.CreateInstance<T>();
        var properties = TypeHelper.Properties(typeof(T));
        foreach (var item in dic)
        {
            var property = properties.FirstOrDefault(p => p.Name.Equals(item.Key, StringComparison.CurrentCultureIgnoreCase));
            if (property != null)
            {
                var value = Utils.ConvertTo(property.PropertyType, item.Value);
                property.SetValue(obj, value);
            }
        }
        if (obj is EntityBase)
        {
            (obj as EntityBase).SetOriginal(dic);
        }
        return obj;
    }

    /// <summary>
    /// 获取导出文件字节数组。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="criteria">查询条件对象。</param>
    /// <param name="pageData">查询结果数据列表。</param>
    /// <param name="onExport">导出扩展字段委托。</param>
    /// <returns>导出文件字节数组。</returns>
    public static byte[] GetExportData<T>(PagingCriteria criteria, List<T> pageData, Func<T, ExportColumnInfo, object> onExport = null)
    {
        if (criteria.ExportColumns == null || criteria.ExportColumns.Count == 0 || pageData.Count == 0)
            return null;

        var excel = ExcelFactory.Create();
        var sheet = excel.CreateSheet("Sheet1");
        var index = 0;
        var headStyle = new StyleInfo { IsBorder = true, IsBold = true, FontColor = Color.White, BackgroundColor = Utils.FromHtml("#6D87C1") };
        foreach (var item in criteria.ExportColumns)
        {
            sheet.SetCellValue(0, index++, item.Name, headStyle);
        }

        var rowIndex = 0;
        var isDictionary = typeof(T) == typeof(Dictionary<string, object>);
        foreach (var data in pageData)
        {
            rowIndex++;
            index = 0;
            foreach (var item in criteria.ExportColumns)
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
                    else if (item.Type == FieldType.Date)
                    {
                        value = Utils.ConvertTo<DateTime?>(value)?.Date;
                        cellStyle.Custom = Config.DateFormat;
                    }
                    else if (item.Type == FieldType.DateTime)
                        cellStyle.Custom = Config.DateTimeFormat;
                    else if (item.Type == FieldType.Number)
                        value = GetNumberValue(value);
                    else if (!string.IsNullOrWhiteSpace(item.Category))
                        value = Cache.GetCodeName(item.Category, value?.ToString());
                }
                sheet.SetCellValue(rowIndex, index++, value, cellStyle);
            }
        }

        var stream = excel.SaveToStream();
        return stream.ToArray();
    }

    private static object GetNumberValue(object value)
    {
        if (decimal.TryParse(value?.ToString(), out var number))
            return number;

        return value;
    }
}