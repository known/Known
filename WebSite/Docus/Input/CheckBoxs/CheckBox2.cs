namespace WebSite.Docus.Input.CheckBoxs;

class CheckBox2 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //默认
        builder.Field<CheckBox>("CheckBox1")
               .Set(f => f.Text, "启用")
               .Set(f => f.Switch, true)
               .Build();
        //开启状态
        builder.Field<CheckBox>("CheckBox2")
               .Set(f => f.Text, "启用")
               .Set(f => f.Switch, true)
               .Set(f => f.Value, "True")
               .Build();
        //禁用状态
        builder.Field<CheckBox>("CheckBox3")
               .Set(f => f.Text, "禁用")
               .Set(f => f.Switch, true)
               .Set(f => f.Enabled, false)
               .Build();
    }
}