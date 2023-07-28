namespace WebSite.Docus.Inputs.CheckBoxs;

class CheckBox4 : BaseComponent
{
    private CheckBox? checkBox;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FieldControl>()
               .Set(c => c.OnVisibleChanged, OnVisibleChanged)
               .Set(c => c.OnEnabledChanged, OnEnabledChanged)
               .Set(c => c.OnReadOnlyChanged, OnReadOnlyChanged)
               .Set(c => c.SetValue, SetValue)
               .Set(c => c.GetValue, GetValue)
               .Build();

        builder.Field<CheckBox>("CheckBox")
               .Set(f => f.Text, "启用")
               .Build(value => checkBox = value);
    }

    private void OnVisibleChanged(bool value) => checkBox?.SetVisible(value);
    private void OnEnabledChanged(bool value) => checkBox?.SetEnabled(value);
    private void OnReadOnlyChanged(bool value) => checkBox?.SetReadOnly(value);
    private void SetValue() => checkBox?.SetValue("True");
    private string? GetValue() => checkBox?.Value;
}