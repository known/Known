namespace Known.AntBlazor.Components;

public class AntRangePicker : RangePicker<DateTime?[]>
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    [Parameter] public string RangeValue { get; set; }
    [Parameter] public Action<string> RangeValueChanged { get; set; }

    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(string);
        base.OnInitialized();
        if (!string.IsNullOrWhiteSpace(RangeValue))
        {
            var values = RangeValue.Split('~');
            if (values.Length > 0)
                Value[0] = Utils.ConvertTo<DateTime?>(values[0]);
            if (values.Length > 1)
                Value[1] = Utils.ConvertTo<DateTime?>(values[1]);
        }
        OnChange = this.Callback<DateRangeChangedEventArgs<DateTime?[]>>(OnDateRangeChange);
    }

    private void OnDateRangeChange(DateRangeChangedEventArgs<DateTime?[]> e)
    {
        RangeValue = $"{e.Dates[0]:yyyy-MM-dd}~{e.Dates[1]:yyyy-MM-dd}";
        RangeValueChanged?.Invoke(RangeValue);
    }
}