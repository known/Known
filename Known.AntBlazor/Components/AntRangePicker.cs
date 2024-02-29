namespace Known.AntBlazor.Components;

public class AntRangePicker<TValue> : RangePicker<DateTime?[]>, IAntField
{
    [Parameter] public int Span { get; set; }
    [Parameter] public string Label { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public TValue RangeValue { get; set; }
    [Parameter] public EventCallback<TValue> RangeValueChanged { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (RangeValue != null)
        {
            var values = RangeValue.ToString().Split('~');
            if (values.Length > 0)
                Value[0] = Utils.ConvertTo<DateTime?>(values[0]);
            if (values.Length > 1)
                Value[1] = Utils.ConvertTo<DateTime?>(values[1]);
        }
        OnChange = this.Callback<DateRangeChangedEventArgs<DateTime?[]>>(OnDateRangeChange);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(Label))
            base.BuildRenderTree(builder);
        else
            builder.FormItem(this, b => base.BuildRenderTree(b));
    }

    private async void OnDateRangeChange(DateRangeChangedEventArgs<DateTime?[]> e)
    {
        RangeValue = Utils.ConvertTo<TValue>($"{e.Dates[0]:yyyy-MM-dd}~{e.Dates[1]:yyyy-MM-dd}");
        if (RangeValueChanged.HasDelegate)
            await RangeValueChanged.InvokeAsync(RangeValue);
    }
}