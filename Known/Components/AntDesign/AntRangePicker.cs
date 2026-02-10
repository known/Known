using AntDesign;
using OneOf;

namespace Known.Components;

/// <summary>
/// 扩展Ant日期范围组件类。
/// </summary>
public class AntRangePicker : RangePicker<DateTime?[]>
{
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 取得或设置表单容器对象。
    /// </summary>
    [CascadingParameter] protected IComContainer AntForm { get; set; }

    /// <summary>
    /// 取得或设置UI上下文对象级联值实例。
    /// </summary>
    [CascadingParameter] public UIContext Context { get; set; }

    /// <summary>
    /// 取得或设置日期范围组件字段绑定值。
    /// </summary>
    [Parameter] public string RangeValue { get; set; }

    /// <summary>
    /// 取得或设置日期范围组件字段绑定值改变事件方法。
    /// </summary>
    [Parameter] public EventCallback<string> RangeValueChanged { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(string);
        base.OnInitialized();
        OnChange = this.Callback<DateRangeChangedEventArgs<DateTime?[]>>(OnDateRangeChange);
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        Value[0] = null;
        Value[1] = null;
        if (!string.IsNullOrWhiteSpace(RangeValue))
        {
            var values = RangeValue.Split('~');
            if (values.Length > 0)
                Value[0] = Utils.ConvertTo<DateTime?>(values[0]);
            if (values.Length > 1)
                Value[1] = Utils.ConvertTo<DateTime?>(values[1]);
        }
        var format = ShowTime.AsT1 == "HH:mm:ss" ? "选择日期时间" : "选择日期";
        Placeholder = OneOf<string, string[]>.FromT1([format, format]);
    }

    private void OnDateRangeChange(DateRangeChangedEventArgs<DateTime?[]> e)
    {
        if (RangeValueChanged.HasDelegate)
        {
            var format = ShowTime.AsT1 == "HH:mm:ss"
                       ? Config.DateTimeFormat
                       : Config.DateFormat;
            var start = e.Dates[0] != null ? e.Dates[0].Value.ToString(format) : "";
            var end = e.Dates[1] != null ? e.Dates[1].Value.ToString(format) : "";
            RangeValue = $"{start}~{end}";
            RangeValueChanged.InvokeAsync(RangeValue);
        }
    }
}