namespace WebSite.Docus.Inputs.TextAreas;

class TextArea1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<TextArea>("默认：", "TextArea1").Build();
        builder.Field<TextArea>("赋值：", "TextArea2").Value("test").Build();
        builder.Field<TextArea>("禁用：", "TextArea3").Enabled(false).Build();
        builder.Field<TextArea>("提示：", "TextArea4").Set(f => f.Placeholder, "输入提示").Build();
    }
}