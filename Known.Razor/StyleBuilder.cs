namespace Known.Razor;

class StyleBuilder
{
    private readonly Dictionary<string, string> styles = new();

    public static StyleBuilder Default() => new();

    public StyleBuilder Add(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
            styles[key] = value;

        return this;
    }

    public StyleBuilder Left(int? left)
    {
        if (left.HasValue)
            styles["left"] = $"{left.Value}px";

        return this;
    }

    public StyleBuilder Width(int? width)
    {
        if (width.HasValue)
            styles["width"] = $"{width.Value}px";

        return this;
    }

    public StyleBuilder Height(int? height)
    {
        if (height.HasValue)
            styles["height"] = $"{height.Value}px";

        return this;
    }

    public string Build() => string.Join(";", styles.Select(s => $"{s.Key}:{s.Value}"));
}

class CssBuilder
{
    private readonly List<string> stringBuffer;

    protected CssBuilder(string value)
    {
        stringBuffer = new List<string>();
        AddClass(value);
    }

    public static CssBuilder Default(string value = null) => new(value);

    public CssBuilder AddClass(string value)
    {
        if (!string.IsNullOrEmpty(value))
            stringBuffer.Add(value);
        return this;
    }

    public CssBuilder AddClass(string value, bool when = true) => when ? AddClass(value) : this;

    public string Build() => stringBuffer.Any() ? string.Join(" ", stringBuffer) : null;
}