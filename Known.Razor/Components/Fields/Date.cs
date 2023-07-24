namespace Known.Razor.Components.Fields;

public class Date : Field
{
    [Parameter] public DateType DateType { get; set; }
    [Parameter] public DateTime? DateValue { get; set; }

    internal override object GetValue()
    {
        if (DateType == DateType.Week || DateType == DateType.Time)
            return Value;

        if (DateType == DateType.Month)
            return DateValue?.ToString("yyyy-MM");

        return DateValue;
    }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        var value = Value;
        var type = string.Empty;
        switch (DateType)
        {
            case DateType.Date:
                value = DateValue?.ToString(Format);
                break;
            case DateType.DateTime:
                value = DateValue?.ToString("yyyy-MM-ddTHH:mm");
                type = "datetime-local";
                break;
            case DateType.Month:
                value = DateValue?.ToString("yyyy-MM");
                break;
            case DateType.Week:
                break;
            case DateType.Time:
                break;
            default:
                break;
        }
        if (DateType != DateType.Week && DateType != DateType.Time)
            BuidDate(builder, Id, value, v => DateValue = v, type);
        else
            BuidDate(builder, Id, value, null, type);
    }

    protected override void SetInputValue(object value)
    {
        if (DateType == DateType.Week || DateType == DateType.Time)
        {
            Value = value?.ToString();
            return;
        }

        DateValue = Utils.ConvertTo<DateTime?>(value);
        Value = DateValue?.ToString(Format);
    }

    protected override string FormatValue(object value)
    {
        if (value == null)
            return string.Empty;

        if (DateType == DateType.Week || DateType == DateType.Time)
            return value?.ToString();

        var date = Utils.ConvertTo<DateTime?>(value);
        return date?.ToString(Format);
    }

    private void BuidDate(RenderTreeBuilder builder, string id, string value, Action<DateTime?> action, string type = null)
    {
        if (string.IsNullOrWhiteSpace(type))
            type = DateType.ToString().ToLower();

        builder.Input(attr =>
        {
            attr.Type(type).Id(id).Name(id).Disabled(!Enabled)
                .Value(value).Required(Required)
                .OnChange(CreateBinder(action));
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
                default:
                    return string.Empty;
            }
        }
    }
}