namespace WebSite.Docus.Nav.Toolbars;

class Toolbar1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KToolbar>()
               .Set(c => c.Tools, DToolbar.Tools)
               .Set(c => c.OnAction, OnAction)
               .Build();
    }

    public void Load() => UI.Toast("点击了加载按钮！");
    public void Edit() => UI.Toast("点击了编辑按钮！");
    public void Save() => UI.Toast("点击了保存按钮！");
    public void Clear() => UI.Toast("点击了清空按钮！");

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