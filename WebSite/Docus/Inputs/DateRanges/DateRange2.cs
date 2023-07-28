namespace WebSite.Docus.Inputs.DateRanges;

class DateRange2 : BaseComponent
{
    private DateRange? dateRange;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FieldControl>()
               .Set(c => c.OnVisibleChanged, OnVisibleChanged)
               .Set(c => c.OnEnabledChanged, OnEnabledChanged)
               .Set(c => c.OnReadOnlyChanged, OnReadOnlyChanged)
               .Set(c => c.SetValue, SetValue)
               .Set(c => c.GetValue, GetValue)
               .Build();

        builder.Field<DateRange>("DateRange").Build(value => dateRange = value);
    }

    private void OnVisibleChanged(bool value) => dateRange?.SetVisible(value);
    private void OnEnabledChanged(bool value) => dateRange?.SetEnabled(value);
    private void OnReadOnlyChanged(bool value) => dateRange?.SetReadOnly(value);
    private void SetValue() => dateRange?.SetValue("2023-01-01~2023-01-31");
    private string? GetValue() => dateRange?.Value;
}