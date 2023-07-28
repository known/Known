namespace Known.Razor.Components.Fields;

public class Date : Field
{
    private bool IsDateValue => DateType == DateType.Date || DateType == DateType.DateTime;

    [Parameter] public DateType DateType { get; set; }

    internal override object GetValue()
    {
        if (!IsDateValue)
            return Value;

        DateTime.TryParseExact(Value, Format, null, DateTimeStyles.None, out DateTime date);
        return date;
    }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        var type = DateType == DateType.DateTime ? "datetime-local" : string.Empty;
        if (IsDateValue)
            BuidDate(builder, Id, Value, type);
        else
            BuidDate(builder, Id, Value, type);
    }

    protected override string FormatValue(object value)
    {
        if (value == null)
            return string.Empty;

        if (value is DateTime time)
            return time.ToString(Format);

        return value?.ToString();
    }

    private void BuidDate(RenderTreeBuilder builder, string id, string value, string type = null)
    {
        if (string.IsNullOrWhiteSpace(type))
            type = DateType.ToString().ToLower();

        builder.Input(attr =>
        {
            attr.Type(type).Id(id).Name(id).Disabled(!Enabled)
                .Value(value).Required(Required)
                .OnChange(CreateBinder());
        });
    }

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
                case DateType.Time:
                    return "HH:mm";
                default:
                    return string.Empty;
            }
        }
    }
}