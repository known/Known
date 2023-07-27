namespace WebSite.Docus.View.Dropdowns;

class Dropdown1 : BaseComponent
{
    private readonly List<MenuItem> items = new()
    {
        new MenuItem("Action1", "操作一", "fa fa-close"),
        new MenuItem("Action2", "操作二", "fa fa-user")
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Dropdown(items, "更多", "button");
    }
}