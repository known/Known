namespace WebSite.Docus.Input.CheckBoxs;

[Title("事件示例")]
class CheckBox3 : BaseComponent
{
    private string? message;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //ValueChanged事件
        builder.Field<CheckBox>("CheckBox1")
               .Set(f => f.Text, "ValueChanged事件")
               .Set(f => f.ValueChanged, OnValueChanged)
               .Build();
        builder.Field<CheckBox>("CheckBox2")
               .Set(f => f.Text, "ValueChanged事件")
               .Set(f => f.Switch, true)
               .Set(f => f.ValueChanged, OnValueChanged)
               .Build();
        builder.Div("tips", message);
    }

    private void OnValueChanged(string value)
    {
        message = value;
        StateChanged();
    }
}