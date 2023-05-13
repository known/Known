namespace Known.Razor.Components.Fields;

public class Date : Input
{
    [Parameter] public bool IsRangeQuery { get; set; } = true;
    [Parameter] public DateTime? Day { get; set; }

    public virtual string Format => Config.DateFormat;

    public override object GetValue() => Day;
    protected override void BuildChildContent(RenderTreeBuilder builder) => BuidDate(builder, Id, Value, v => Day = v);

    protected override void SetInputValue(object value)
    {
        Day = Utils.ConvertTo<DateTime?>(value);
        Value = Day?.ToString(Format);
    }

    protected override string FormatValue(object value)
    {
        if (value == null)
            return string.Empty;

        var date = Utils.ConvertTo<DateTime?>(value);
        return date?.ToString(Format);
    }
}

public class DateTimeL : Date
{
    public override string Format => "yyyy-MM-dd HH:mm";

    protected override void BuildChildContent(RenderTreeBuilder builder)
    {
        var value = Day?.ToString("yyyy-MM-ddTHH:mm");
        BuidDate(builder, Id, value, v => Day = v, "datetime-local");
    }
}

public class DateMonth : Date
{
    public override string Format => "yyyy-MM";

    public override object GetValue() => Day?.ToString("yyyy-MM");

    protected override void BuildChildContent(RenderTreeBuilder builder)
    {
        var value = Day?.ToString("yyyy-MM");
        BuidDate(builder, Id, value, v => Day = v, "month");
    }
}

public class DateRange : Input
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

    protected override void BuildChildContent(RenderTreeBuilder builder)
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

    private void SetValue(DateTime? date, int index)
    {
        values[index] = date?.ToString(format);
        Value = string.Join(split, values);
    }
}