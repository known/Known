namespace WebSite.Docus.Input.CheckBoxs;

[Title("默认示例")]
class CheckBox1 : BaseComponent
{
    private string? message;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //默认
        builder.Field<CheckBox>("CheckBox")
               .Set(f => f.Text, "启用")
               .Build();
        //选中状态
        builder.Field<CheckBox>("CheckBox")
               .Set(f => f.Text, "启用")
               .Set(f => f.Value, "True")
               .Build();
        //带ValueChanged事件
        builder.Field<CheckBox>("CheckBox")
               .Set(f => f.Text, "ValueChanged事件")
               .Set(f => f.ValueChanged, OnValueChanged)
               .Build();
        //禁用状态
        builder.Field<CheckBox>("CheckBox")
               .Set(f => f.Text, "禁用")
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