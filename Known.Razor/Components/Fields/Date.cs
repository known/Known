namespace Known.Razor.Components.Fields;

public enum DateType { Date, DateTime, Month }

public class Date : Input
{
    [Parameter] public DateType DateType { get; set; }
    [Parameter] public DateTime? DateValue { get; set; }

    private string Format
    {
        get
        {
            switch (DateType)
            {
                case DateType.Date:
                    return Config.DateFormat;
                case DateType.DateTime:
                    return "yyyy-MM-dd HH:mm";
                case DateType.Month:
                    return "yyyy-MM";
                default:
                    return Config.DateFormat;
            }
        }
    }

    public override object GetValue()
    {
        if (DateType == DateType.Month)
            return DateValue?.ToString("yyyy-MM");
        else
            return DateValue;
    }

    protected override void BuildChildContent(RenderTreeBuilder builder)
    {
        if (DateType == DateType.DateTime)
        {
            var value = DateValue?.ToString("yyyy-MM-ddTHH:mm");
            BuidDate(builder, Id, value, v => DateValue = v, "datetime-local");
        }
        else if (DateType == DateType.Month)
        {
            var value = DateValue?.ToString("yyyy-MM");
            BuidDate(builder, Id, value, v => DateValue = v, "month");
        }
        else
        {
            BuidDate(builder, Id, DateValue?.ToString(Format), v => DateValue = v);
        }
    }

    protected override void SetInputValue(object value)
    {
        DateValue = Utils.ConvertTo<DateTime?>(value);
        Value = DateValue?.ToString(Format);
    }

    protected override string FormatValue(object value)
    {
        if (value == null)
            return string.Empty;

        var date = Utils.ConvertTo<DateTime?>(value);
        return date?.ToString(Format);
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