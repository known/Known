namespace WebSite.Docus.Input.CheckBoxs;

class CheckBox4 : BaseComponent
{
    private CheckBox? checkBox;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FieldControl>()
               .Set(c => c.OnVisibleChanged, OnVisibleChanged)
               .Set(c => c.OnEnabledChanged, OnEnabledChanged)
               .Set(c => c.SetValue, SetValue)
               .Set(c => c.GetValue, GetValue)
               .Build();

        builder.Field<CheckBox>("CheckBox")
               .Set(f => f.Text, "启用")
               .Build(value => checkBox = value);
    }

    private void OnVisibleChanged(string value)
    {
        var visible = Utils.ConvertTo<bool>(value);
        checkBox?.SetVisible(visible);
    }

    private void OnEnabledChanged(string value)
    {
        var enabled = Utils.ConvertTo<bool>(value);
        checkBox?.SetEnabled(enabled);
    }

    private void SetValue() => checkBox?.SetValue("True");

    private string? GetValue() => checkBox?.Value;
}