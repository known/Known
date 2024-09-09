namespace Known.Blazor;

/// <summary>
/// CSS样式建造者类。
/// </summary>
public class CssBuilder
{
    private readonly List<string> classes = [];
    private readonly List<string> styles = [];
    private readonly Dictionary<string, string> dicStyles = [];

    /// <summary>
    /// 构造函数，创建一个CSS样式建造者类的实例。
    /// </summary>
    /// <param name="className">CSS类名。</param>
    protected CssBuilder(string className)
    {
        AddClass(className);
    }

    /// <summary>
    /// 创建默认CSS样式建造者。
    /// </summary>
    /// <param name="className">CSS类名。</param>
    /// <returns>CSS样式建造者。</returns>
    public static CssBuilder Default(string className = null) => new(className);

    /// <summary>
    /// 添加Style样式键值对。
    /// </summary>
    /// <param name="key">样式键。</param>
    /// <param name="value">样式值。</param>
    /// <returns>CSS样式建造者。</returns>
    public CssBuilder Add(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
            dicStyles[key] = value;

        return this;
    }

    /// <summary>
    /// 添加Style样式字符串。
    /// </summary>
    /// <param name="style">样式字符串。</param>
    /// <returns>CSS样式建造者。</returns>
    public CssBuilder AddStyle(string style)
    {
        if (!string.IsNullOrEmpty(style))
            styles.Add(style);

        return this;
    }

    /// <summary>
    /// 添加CSS类名。
    /// </summary>
    /// <param name="className">CSS类名。</param>
    /// <returns>CSS样式建造者。</returns>
    public CssBuilder AddClass(string className)
    {
        if (!string.IsNullOrEmpty(className))
            classes.Add(className);

        return this;
    }

    /// <summary>
    /// 添加满足条件的CSS类名。
    /// </summary>
    /// <param name="className">CSS类名。</param>
    /// <param name="when">是否满足条件。</param>
    /// <returns>CSS样式建造者。</returns>
    public CssBuilder AddClass(string className, bool when) => when ? AddClass(className) : this;

    /// <summary>
    /// 建造CSS类名字符串。
    /// </summary>
    /// <returns>CSS类名。</returns>
    public string BuildClass() => classes.Count != 0 ? string.Join(" ", classes) : null;

    /// <summary>
    /// 建造Style样式字符串。
    /// </summary>
    /// <returns>Style样式字符串。</returns>
    public string BuildStyle()
    {
        var style = string.Empty;
        if (dicStyles != null && dicStyles.Count > 0)
            style += string.Join(";", dicStyles.Select(s => $"{s.Key}:{s.Value}")) + ";";
        if (styles != null && styles.Count > 0)
        {
            foreach (var item in styles)
            {
                var itemStyle = item.EndsWith(';') ? item : $"{item};";
                style += itemStyle;
            }
        }
        return style;
    }
}