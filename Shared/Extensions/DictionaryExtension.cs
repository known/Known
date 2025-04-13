namespace Known.Extensions;

/// <summary>
/// 字典扩展类。
/// </summary>
public static class DictionaryExtension
{
    /// <summary>
    /// 判断类型是否是字典（Dictionary[string, object]）。
    /// </summary>
    /// <param name="type">类型。</param>
    /// <returns></returns>
    public static bool IsDictionary(this Type type)
    {
        return type == typeof(Dictionary<string, object>);
    }

    /// <summary>
    /// 获取金额字符串。
    /// </summary>
    /// <param name="dic">字典对象。</param>
    /// <param name="key">字典项目键。</param>
    /// <returns>金额字符串。</returns>
    public static string GetAmount(this IDictionary dic, string key)
    {
        var amount = dic?.GetValue<decimal>(key);
        if (amount == null || amount == 0)
            return "0";

        return amount.ToString().TrimEnd('0').TrimEnd('.');
    }

    /// <summary>
    /// 获取字典项目值。
    /// </summary>
    /// <param name="dic">字典对象。</param>
    /// <param name="key">字典项目键。</param>
    /// <returns>字典项目值。</returns>
    public static object GetValue(this IDictionary dic, string key)
    {
        if (dic == null)
            return null;

        if (string.IsNullOrWhiteSpace(key))
            return null;

        if (dic.Contains(key))
            return dic[key];

        if (dic.Contains(key.ToLower()))
            return dic[key.ToLower()];

        if (dic.Contains(key.ToUpper()))
            return dic[key.ToUpper()];

        return null;
    }

    /// <summary>
    /// 获取字典项目泛型值。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="dic">字典对象。</param>
    /// <param name="key">字典项目键。</param>
    /// <returns>字典项目泛型值。</returns>
    public static T GetValue<T>(this IDictionary dic, string key)
    {
        var value = dic?.GetValue(key);
        if (value == null)
            return default;

        return Utils.ConvertTo<T>(value);
    }

    /// <summary>
    /// 获取字典项目指定类型值。
    /// </summary>
    /// <param name="dic">字典对象。</param>
    /// <param name="type">指定类型。</param>
    /// <param name="key">字典项目键。</param>
    /// <returns>字典项目指定类型值。</returns>
    public static object GetValue(this IDictionary dic, Type type, string key)
    {
        var value = dic?.GetValue(key);
        if (value == null)
            return default;

        return Utils.ConvertTo(type, value);
    }

    /// <summary>
    /// 设置无代码字典对象字段值。
    /// </summary>
    /// <param name="dic">字典对象。</param>
    /// <param name="id">字段ID。</param>
    /// <param name="value">字段值。</param>
    public static void SetValue(this IDictionary dic, string id, object value)
    {
        var key = id;
        if (dic.Contains(id.ToLower()))
            key = id.ToLower();
        else if (dic.Contains(id.ToUpper()))
            key = id.ToUpper();
        dic[key] = value;
    }

    /// <summary>
    /// 根据字典对象自动获取表格栏位列表。
    /// </summary>
    /// <param name="value">字典对象</param>
    /// <returns></returns>
    public static List<ColumnInfo> GetColumns(this Dictionary<string, object> value)
    {
        var columns = new List<ColumnInfo>();
        foreach (var item in value)
        {
            var info = new ColumnInfo { Id = item.Key, Name = item.Key };
            if (item.Value != null)
            {
                var type = item.Value.GetType();
                info.Type = type.GetFieldType();
                info.Width = type.GetColumnWidth();
            }
            columns.Add(info);
        }
        return columns;
    }

    /// <summary>
    /// 替换数据占位符。
    /// </summary>
    /// <param name="data">数据字典。</param>
    /// <param name="user">当前用户。</param>
    public static void ReplaceDataPlaceholder(this Dictionary<string, object> data, UserInfo user)
    {
        foreach (var key in data.Keys)
        {
            var value = data[key];
            if (DataPlaceholder.CurrentCompNo.Equals(value))
                data[key] = user.CompNo;
            if (DataPlaceholder.CurrentUser.Equals(value))
                data[key] = user.UserName;
            if (DataPlaceholder.CurrentTime.Equals(value))
                data[key] = DateTime.Now;
        }
    }
}