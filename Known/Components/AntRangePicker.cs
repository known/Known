﻿using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant日期范围组件类。
/// </summary>
public class AntRangePicker : RangePicker<DateTime?[]>
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 取得或设置日期范围组件字段绑定值。
    /// </summary>
    [Parameter] public string RangeValue { get; set; }

    /// <summary>
    /// 取得或设置日期范围组件字段绑定值改变事件方法。
    /// </summary>
    [Parameter] public Action<string> RangeValueChanged { get; set; }

    /// <summary>
    /// 初始化组件。
    /// </summary>
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(string);
        base.OnInitialized();
        OnChange = this.Callback<DateRangeChangedEventArgs<DateTime?[]>>(OnDateRangeChange);
    }

    /// <summary>
    /// 异步设置组件参数。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (!string.IsNullOrWhiteSpace(RangeValue))
        {
            var values = RangeValue.Split('~');
            if (values.Length > 0)
                Value[0] = Utils.ConvertTo<DateTime?>(values[0]);
            if (values.Length > 1)
                Value[1] = Utils.ConvertTo<DateTime?>(values[1]);
        }
    }

    private void OnDateRangeChange(DateRangeChangedEventArgs<DateTime?[]> e)
    {
        RangeValue = $"{e.Dates[0]:yyyy-MM-dd}~{e.Dates[1]:yyyy-MM-dd}";
        RangeValueChanged?.Invoke(RangeValue);
    }
}