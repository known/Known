namespace Test.Razor.Samples;

class DemoOther : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<SearchBox>().Build();
    }
}