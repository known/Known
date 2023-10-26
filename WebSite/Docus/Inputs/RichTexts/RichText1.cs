namespace WebSite.Docus.Inputs.RichTexts;

class RichText1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<KRichText>("内容：", "RichText1").Value("<h1>孙膑是辅助和法师。</h1>")
               .Set(f => f.Option, new
               {
                   Height = 200,
                   Placeholder = "请输入通知内容"
               })
               .Build();
    }
}