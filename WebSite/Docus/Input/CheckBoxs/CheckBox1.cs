namespace WebSite.Docus.Input.CheckBoxs;

class CheckBox1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<CheckBox>("CheckBox1").Set(f => f.Text, "默认").Build();
        builder.Field<CheckBox>("CheckBox2").Value("True").Set(f => f.Text, "启用").Build();
        builder.Field<CheckBox>("CheckBox3").Enabled(false).Set(f => f.Text, "禁用").Build();
    }
}