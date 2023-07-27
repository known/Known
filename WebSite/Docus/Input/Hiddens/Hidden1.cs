namespace WebSite.Docus.Input.Hiddens;

class Hidden1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<Hidden>("Id");
    }
}