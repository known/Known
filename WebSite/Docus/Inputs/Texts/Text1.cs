namespace WebSite.Docus.Inputs.Texts;

class Text1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<Text>("默认：", "Text1").Build();
        builder.Field<Text>("赋值：", "Text2").Value("test").Build();
        builder.Field<Text>("禁用：", "Text3").Enabled(false).Build();
        builder.Field<Text>("提示：", "Text4").Set(f => f.Placeholder, "输入提示").Build();
    }
}