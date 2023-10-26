namespace WebSite.Docus;

class BaseDocu : BaseComponent
{
    [Parameter] public MenuItem? Item { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.H1($"{Item?.Name} ({Item?.Id})");
        builder.Div("doc-desc", Item?.Description);

        builder.H2("概述");
        BuildOverview(builder);

        builder.H2("代码演示");
        BuildCodeDemo(builder);
    }

    protected virtual void BuildOverview(RenderTreeBuilder builder) { }
    protected virtual void BuildCodeDemo(RenderTreeBuilder builder) { }
}