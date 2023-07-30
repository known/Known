namespace Known.Razor.Components.Fields;

public class DateRange : Field
{
    private readonly string split = "~";
    private readonly string[] values = new string[] { "", "" };
    private string startId;
    private string endId;

    public DateRange()
    {
        Split = split;
    }

    [Parameter] public string Split { get; set; }

    internal override object GetFieldValue() => string.Join(split, values);

    protected override void OnInitialized()
    {
        base.OnInitialized();
        FormatValue(Value);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        startId = $"L{Id}";
        endId = $"G{Id}";
    }

    protected override string FormatValue(object value)
    {
        var tmpValues = value?.ToString()?.Split(Split);
        if (tmpValues == null)
            return string.Empty;

        if (tmpValues.Length > 0)
            values[0] = tmpValues[0];
        if (tmpValues.Length > 1)
            values[1] = tmpValues[1];

        return string.Join(split, values);
    }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        BuidDate(builder, startId, values[0], value => SetValue(value, 0));
        if (!string.IsNullOrWhiteSpace(Split))
            builder.Span(attr => builder.Text(Split));
        BuidDate(builder, endId, values[1], value => SetValue(value, 1));
    }

    private void BuidDate(RenderTreeBuilder builder, string id, string value, Action<string> action)
    {
        builder.Input(attr =>
        {
            attr.Type("date").Id(id).Name(id).Disabled(!Enabled)
                .Value(value).Required(Required)
                .OnChange(EventCallback.Factory.CreateBinder(this, action, value));
        });
    }

    private void SetValue(string value, int index)
    {
        values[index] = value;
        Value = string.Join(split, values);
        OnValueChange();
    }
}