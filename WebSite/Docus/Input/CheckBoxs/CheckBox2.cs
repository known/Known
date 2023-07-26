namespace WebSite.Docus.Input.CheckBoxs;

[Title("开关示例")]
class CheckBox2 : BaseComponent
{
    private string? message;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //默认
        builder.Field<CheckBox>("CheckBox")
               .Set(f => f.Text, "启用")
               .Set(f => f.Switch, true)
               .Build();
        //开启状态
        builder.Field<CheckBox>("CheckBox")
               .Set(f => f.Text, "启用")
               .Set(f => f.Switch, true)
               .Set(f => f.Value, "True")
               .Build();
        //带ValueChanged事件
        builder.Field<CheckBox>("CheckBox")
               .Set(f => f.Text, "ValueChanged事件")
               .Set(f => f.Switch, true)
               .Set(f => f.ValueChanged, OnValueChanged)
               .Build();
        //禁用状态
        builder.Field<CheckBox>("CheckBox")
               .Set(f => f.Text, "禁用")
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