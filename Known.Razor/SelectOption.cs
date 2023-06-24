namespace Known.Razor;

public class SelectOption
{
    public SelectOption() { }

    public SelectOption(string codes)
    {
        Codes = codes;
    }

    public SelectOption(Type enumType)
    {
        ShowEmpty = false;
        Items = TypeHelper.GetEnumCodes(enumType).ToArray();
    }

    public bool ShowEmpty { get; set; } = true;
    public string Codes { get; set; }
    public CodeInfo[] Items { get; set; }
    public Action<string> ValueChanged { get; set; }

    internal string Format(object value)
    {
        var codes = Items;
        if (codes == null || codes.Length == 0)
            codes = CodeInfo.GetCodes(Codes).ToArray();

        var str = value?.ToString();
        return codes.FirstOrDefault(i => i.Code == str)?.Name;
    }

    internal void BuildQuery(RenderTreeBuilder builder, ColumnInfo column, string name, string value, Action<string> valueChanged = null, Action refresh = null)
    {
        var emptyText = ShowEmpty ? "全部" : "";
        builder.Field<Select>(name, column.Id).IsInput(true).Value(value)
               .ValueChanged(value =>
               {
                   valueChanged?.Invoke(value);
                   ValueChanged?.Invoke(value);
                   refresh?.Invoke();
               })
               .Set(c => c.EmptyText, emptyText)
               .Set(f => f.Codes, Codes)
               .Set(f => f.Items, Items)
               .Build();
    }

    internal void BuildCell(RenderTreeBuilder builder, string id, string value, Action<string> valueChanged = null)
    {
        var emptyText = ShowEmpty ? "请选择" : "";
        builder.Field<Select>(id).Value(value).IsInput(true)
               .ValueChanged(value =>
               {
                   valueChanged?.Invoke(value);
                   ValueChanged?.Invoke(value);
               })
               .Set(c => c.EmptyText, emptyText)
               .Set(f => f.Codes, Codes)
               .Set(f => f.Items, Items)
               .Build();
    }
}

public class SelectOption<T> : SelectOption
{
    public SelectOption() { }

    public SelectOption(string codes, Action<T, object> valueChanged = null) : base(codes)
    {
        ValueChanged = valueChanged;
    }

    public SelectOption(Type enumType, Action<T, object> valueChanged = null) : base(enumType)
    {
        ValueChanged = valueChanged;
    }

    public new Action<T, object> ValueChanged { get; set; }
}