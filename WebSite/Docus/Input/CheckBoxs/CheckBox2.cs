namespace WebSite.Docus.Input.CheckBoxs;

[Title("开关示例")]
class CheckBox2 : BaseComponent
{
    private string? message;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<CheckBox>("CheckBox")
               .Set(f => f.Switch, true)
               .Set(f => f.ValueChanged, OnValueChanged)
               .Build();
        builder.Field<CheckBox>("CheckBox")
               .Set(f => f.Switch, true)
               .Set(f => f.Enabled, false)
               .Build();
        builder.Div("tips", message);
    }

    private void OnValueChanged(string value)
    {
        message = value;
        StateChanged();
    }
}