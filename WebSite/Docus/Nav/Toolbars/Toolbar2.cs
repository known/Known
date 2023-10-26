namespace WebSite.Docus.Nav.Toolbars;

class Toolbar2 : BaseComponent
{
    private readonly List<ButtonInfo> tools = new();

    public Toolbar2()
    {
        tools.Add(new ButtonInfo("Edit", "编辑", "fa fa-file-o", StyleType.Success));
        tools.Add(new ButtonInfo("Clear", "清空", "fa fa-trash-o", StyleType.Danger));
        var more = new ButtonInfo("More", "更多", "");
        more.Children.Add(new ButtonInfo("More1", "操作一", "fa fa-close"));
        more.Children.Add(new ButtonInfo("More2", "操作二", "fa fa-user") { Enabled = false });
        more.Children.Add(new ButtonInfo("More3", "操作三", "fa fa-cog"));
        tools.Add(more);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KToolbar>()
               .Set(c => c.Tools, tools)
               .Set(c => c.OnAction, OnAction)
               .Build();
    }

    private void OnAction(ButtonInfo info)
    {
        var method = GetType().GetMethod(info.Id);
        if (method == null)
            UI.Toast($"{info.Name}方法不存在！", StyleType.Danger);
        else
            method.Invoke(this, null);
        StateChanged();
    }
}