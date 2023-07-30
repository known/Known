namespace WebSite.Docus.Inputs.RichTexts;

class RichText1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<RichText>("内容：", "RichText1").Build();
    }
}