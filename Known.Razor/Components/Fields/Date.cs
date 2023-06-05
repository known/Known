namespace Known.Razor.Components.Fields;

public enum DateType { Date, DateTime, Month }

public class Date : Field
{
    [Parameter] public DateType DateType { get; set; }
    [Parameter] public DateTime? DateValue { get; set; }

    internal override object GetValue()
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

    private void BuidDate(RenderTreeBuilder builder, string id, string value, Action<DateTime?> action, string type = "date")
    {
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
                    return Config.DateFormat;
            }
        }
    }
}