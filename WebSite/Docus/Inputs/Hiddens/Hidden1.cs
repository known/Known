namespace WebSite.Docus.Inputs.Hiddens;

class Hidden1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<KHidden>("Id");
    }
}