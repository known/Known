namespace Known.Razor.Components.Fields;

public class DateRange : Field
{
    private readonly string format = Config.DateFormat;
    private readonly string split = "~";
    private readonly string[] values = new string[] { "", "" };
    private string startId;
    private string endId;

    public DateRange()
    {
        Split = split;
    }

    [Parameter] public string Split { get; set; }
    [Parameter] public DateTime? Start { get; set; }
    [Parameter] public DateTime? End { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        startId = $"L{Id}";
        endId = $"G{Id}";
        SetValue(Start, 0);
        SetValue(End, 1);
    }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        var start = Start?.ToString(format);
        var end = End?.ToString(format);

        BuidDate(builder, startId, start, v =>
        {
            SetValue(v, 0);
            Start = v;
        });
        if (!string.IsNullOrWhiteSpace(Split))
            builder.Span(attr => builder.Text(Split));
        BuidDate(builder, endId, end, v =>
        {
            SetValue(v, 1);
            End = v;
        });
    }

    private void BuidDate(RenderTreeBuilder builder, string id, string value, Action<DateTime?> action, string type = "date")
    {
        builder.Input(attr =>
        {
            attr.Type(type).Id(id).Name(id).Disabled(!Enabled)
                .Value(value).Required(Required)
                .OnChange(CreateBinder(action));
        });
    }

    private void SetValue(DateTime? date, int index)
    {
        values[index] = date?.ToString(format);
        Value = string.Join(split, values);
    }
}