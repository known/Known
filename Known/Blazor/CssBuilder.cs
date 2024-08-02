namespace Known.Blazor;

public class CssBuilder
{
    private readonly List<string> classes = [];
    private readonly List<string> styles = [];
    private readonly Dictionary<string, string> dicStyles = [];

    protected CssBuilder(string className)
    {
        AddClass(className);
    }

    public static CssBuilder Default(string className = null) => new(className);

    public CssBuilder Add(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
            dicStyles[key] = value;

        return this;
    }

    public CssBuilder AddStyle(string style)
    {
        if (!string.IsNullOrEmpty(style))
            styles.Add(style);

        return this;
    }

    public CssBuilder AddClass(string className)
    {
        if (!string.IsNullOrEmpty(className))
            classes.Add(className);

        return this;
    }

    public CssBuilder AddClass(string className, bool when) => when ? AddClass(className) : this;
    public string BuildClass() => classes.Count != 0 ? string.Join(" ", classes) : null;

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