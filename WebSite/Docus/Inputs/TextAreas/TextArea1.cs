namespace WebSite.Docus.Inputs.TextAreas;

class TextArea1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<KTextArea>("默认：", "TextArea1").Build();
        builder.Field<KTextArea>("赋值：", "TextArea2").Value("test").Build();
        builder.Field<KTextArea>("禁用：", "TextArea3").Enabled(false).Build();
        builder.Field<KTextArea>("提示：", "TextArea4").Set(f => f.Placeholder, "输入提示").Build();
    }
}