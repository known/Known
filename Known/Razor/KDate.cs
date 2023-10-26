namespace Known.Razor;

public class KDate : Field
{
    private bool IsDateValue => DateType == DateType.Date || DateType == DateType.DateTime;

    [Parameter] public DateType DateType { get; set; }

    internal override object GetFieldValue()
    {
        if (string.IsNullOrWhiteSpace(Value))
            return null;

        if (!IsDateValue)
            return Value;

        DateTime.TryParseExact(Value, Format, null, DateTimeStyles.None, out DateTime date);
        return date;
    }

    protected override string FormatValue(object value)
    {
        if (value == null)
            return string.Empty;

        if (value is DateTime time)
            return time.ToString(Format);

        if (IsDateValue)
        {
            var date = Utils.ConvertTo<DateTime>(value);
            return date.ToString(Format);
        }

        return value?.ToString();
    }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        var type = DateType == DateType.DateTime
                 ? "datetime-local"
                 : DateType.ToString().ToLower();
        builder.Input(attr =>
        {
            attr.Type(type).Id(Id).Name(Id).Disabled(!Enabled)
                .Value(Value).Required(Required)
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