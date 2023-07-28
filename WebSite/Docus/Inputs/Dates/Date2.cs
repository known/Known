namespace WebSite.Docus.Inputs.Dates;

class Date2 : BaseComponent
{
    private Date? date;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FieldControl>()
               .Set(c => c.OnVisibleChanged, OnVisibleChanged)
               .Set(c => c.OnEnabledChanged, OnEnabledChanged)
               .Set(c => c.OnReadOnlyChanged, OnReadOnlyChanged)
               .Set(c => c.SetValue, SetValue)
               .Set(c => c.GetValue, GetValue)
               .Build();

        builder.Field<Date>("日期时间：", "DateTime")
               .Set(f => f.DateType, DateType.DateTime)
               .Build(value => date = value);
    }

    private void OnVisibleChanged(bool value) => date?.SetVisible(value);
    private void OnEnabledChanged(bool value) => date?.SetEnabled(value);
    private void OnReadOnlyChanged(bool value) => date?.SetReadOnly(value);
    private void SetValue() => date?.SetValue(DateTime.Now);
    private string? GetValue() => date?.Value;
}