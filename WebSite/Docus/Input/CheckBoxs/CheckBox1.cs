namespace WebSite.Docus.Input.CheckBoxs;

[Title("默认示例")]
class CheckBox1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //默认
        builder.Field<CheckBox>("CheckBox1")
               .Set(f => f.Text, "启用")
               .Build();
        //选中状态
        builder.Field<CheckBox>("CheckBox2")
               .Set(f => f.Text, "启用")
               .Set(f => f.Value, "True")
               .Build();
        //禁用状态
        builder.Field<CheckBox>("CheckBox3")
               .Set(f => f.Text, "禁用")
               .Set(f => f.Enabled, false)
               .Build();
    }
}