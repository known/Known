namespace Known;

/// <summary>
/// 数据占位符。
/// </summary>
public class DataPlaceholder
{
    private DataPlaceholder() { }

    static DataPlaceholder()
    {
        Add("CompNo", "当前公司编码", u => u.CompNo);
        Add("UserName", "当前用户名", u => u.UserName);
        Add("Now", "当前时间", u => DateTime.Now);
    }

    /// <summary>
    /// 取得数据占位符信息列表。
    /// </summary>
    public static List<DataPlaceholder> Placeholders { get; } = [];

    /// <summary>
    /// 取得数据占位符代码信息列表。
    /// </summary>
    public static List<CodeInfo> Codes => [.. Placeholders.Select(d => new CodeInfo(d.Key, d.Name))];

    /// <summary>
    /// 取得或设置键值。
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 取得或设置显示名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置格式化委托。
    /// </summary>
    public Func<UserInfo, object> Format { get; set; }

    /// <summary>
    /// 添加数据占位符。
    /// </summary>
    /// <param name="key">键值。</param>
    /// <param name="name">显示名称。</param>
    /// <param name="format">格式化委托。</param>
    public static void Add(string key, string name, Func<UserInfo, object> format)
    {
        var id = $"{{{key}}}";
        if (Placeholders.Exists(d => d.Key == id))
            return;

        Placeholders.Add(new DataPlaceholder { Key = id, Name = name, Format = format });
    }

    /// <summary>
    /// 格式化占位符对象值。
    /// </summary>
    /// <param name="value">占位符。</param>
    /// <param name="user">当前用户。</param>
    /// <returns></returns>
    public static object FormatValue(string value, UserInfo user)
    {
        var data = Placeholders.FirstOrDefault(d => d.Key == value);
        if (data == null)
            return value;

        return data.Format?.Invoke(user);
    }
}