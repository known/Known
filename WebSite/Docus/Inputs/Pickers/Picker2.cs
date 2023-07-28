namespace WebSite.Docus.Inputs.Pickers;

class Picker2 : BaseComponent
{
    private Picker? picker;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FieldControl>()
               .Set(c => c.OnVisibleChanged, OnVisibleChanged)
               .Set(c => c.OnEnabledChanged, OnEnabledChanged)
               .Set(c => c.OnReadOnlyChanged, OnReadOnlyChanged)
               .Set(c => c.SetValue, SetValue)
               .Set(c => c.GetValue, GetValue)
               .Build();

        builder.Field<Picker>("客户", "Picker")
               .Set(f => f.Pick, new CustomerList())
               .Build(value => picker = value);
    }

    private void OnVisibleChanged(bool value) => picker?.SetVisible(value);
    private void OnEnabledChanged(bool value) => picker?.SetEnabled(value);
    private void OnReadOnlyChanged(bool value) => picker?.SetReadOnly(value);
    private void SetValue() => picker?.SetValue("test");
    private string? GetValue() => picker?.Value;
}