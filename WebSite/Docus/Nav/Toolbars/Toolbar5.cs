namespace WebSite.Docus.Nav.Toolbars;

class Toolbar5 : BaseComponent
{
    private readonly List<ButtonInfo> tools = new()
    {
        new ButtonInfo("Open", "打开", "", StyleType.Primary)
    };
    private Toolbar? toolbar;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Toolbar>()
               .Set(c => c.Tools, tools)
               .Set(c => c.OnAction, OnAction)
               .Build(value => toolbar = value);
    }

    private void OnAction(ButtonInfo info)
    {
        if (info.Id == "Open")
        {
            toolbar?.SetItemName("Open", info.Name == "打开" ? "关闭" : "打开");
        }
    }
}