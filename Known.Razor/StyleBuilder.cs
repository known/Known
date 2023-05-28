namespace Known.Razor;

class StyleBuilder
{
    private readonly Dictionary<string, string> styles = new();

    internal static StyleBuilder Default => new();

    internal StyleBuilder Add(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
            styles[key] = value;

        return this;
    }

    internal StyleBuilder Left(int? left)
    {
        if (left.HasValue)
            styles["left"] = $"{left.Value}px";

        return this;
    }

    internal StyleBuilder Width(int? width)
    {
        if (width.HasValue)
            styles["width"] = $"{width.Value}px";

        return this;
    }

    internal StyleBuilder Height(int? height)
    {
        if (height.HasValue)
            styles["height"] = $"{height.Value}px";

        return this;
    }

    internal string Build() => string.Join(";", styles.Select(s => $"{s.Key}:{s.Value}"));
}

public class CssBuilder
{
    private readonly List<string> sb = new();

    protected CssBuilder(string value)
    {
        AddClass(value);
    }

    public static CssBuilder Default(string value = null) => new(value);

    public CssBuilder AddClass(string value)
    {
        if (!string.IsNullOrEmpty(value))
            sb.Add(value);

        return this;
    }

    public CssBuilder AddClass(string value, bool when = true) => when ? AddClass(value) : this;
    public string Build() => sb.Any() ? string.Join(" ", sb) : null;
}