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
        builder.Field<CheckBox>("CheckBox2").Value("True")
               .Set(f => f.Text, "启用")
               .Set(f => f.Switch, true)
               .Build();
        //禁用状态
        builder.Field<CheckBox>("CheckBox3").Enabled(false)
               .Set(f => f.Text, "禁用")
               .Set(f => f.Switch, true)
               .Build();
    }
}