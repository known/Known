namespace WebSite.Docus.Inputs.Texts;

class Text1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<KText>("默认：", "Text1").Build();
        builder.Field<KText>("赋值：", "Text2").Value("test").Build();
        builder.Field<KText>("禁用：", "Text3").Enabled(false).Build();
        builder.Field<KText>("提示：", "Text4").Set(f => f.Placeholder, "输入提示").Build();
    }
}