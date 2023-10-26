namespace WebSite.Docus.Inputs.CheckBoxs;

class CheckBox1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<KCheckBox>("CheckBox1").Set(f => f.Text, "默认").Build();
        builder.Field<KCheckBox>("CheckBox2").Value("True").Set(f => f.Text, "启用").Build();
        builder.Field<KCheckBox>("CheckBox3").Enabled(false).Set(f => f.Text, "禁用").Build();
    }
}