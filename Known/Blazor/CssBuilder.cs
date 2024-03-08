namespace Known.Blazor;

public class CssBuilder
{
    private readonly List<string> classes = [];
    private readonly Dictionary<string, string> styles = [];

    protected CssBuilder(string className)
    {
        AddClass(className);
    }

    public static CssBuilder Default(string className = null) => new(className);

    public CssBuilder Add(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
            styles[key] = value;

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
    public string BuildStyle() => string.Join(";", styles.Select(s => $"{s.Key}:{s.Value}"));
}