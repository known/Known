namespace WebSite.Docus.Inputs.Numbers;

class Number1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<Number>("数量1：", "Number1").Build();
        builder.Field<Number>("数量2：", "Number2").Value("10").Set(f => f.Unit, "个").Build();
    }
}