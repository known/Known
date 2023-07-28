namespace WebSite.Docus.Inputs.Numbers;

class Number2 : BaseComponent
{
    private Number? number;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FieldControl>()
               .Set(c => c.OnVisibleChanged, OnVisibleChanged)
               .Set(c => c.OnEnabledChanged, OnEnabledChanged)
               .Set(c => c.OnReadOnlyChanged, OnReadOnlyChanged)
               .Set(c => c.SetValue, SetValue)
               .Set(c => c.GetValue, GetValue)
               .Build();

        builder.Field<Number>("数量", "Number").Set(f => f.Unit, "个").Build(value => number = value);
    }

    private void OnVisibleChanged(bool value) => number?.SetVisible(value);
    private void OnEnabledChanged(bool value) => number?.SetEnabled(value);
    private void OnReadOnlyChanged(bool value) => number?.SetReadOnly(value);
    private void SetValue() => number?.SetValue(100);
    private string? GetValue() => number?.Value;
}