namespace WebSite.Docus.Inputs.Numbers;

class Number1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<Number>("数量", "Number1").Build();
        builder.Field<Number>("数量", "Number2").Set(f => f.Unit, "个").Build();
    }
}