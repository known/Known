namespace WebSite.Docus.Nav.Toolbars;

class Toolbar3 : BaseComponent
{
    private KToolbar? toolbar;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KToolbar>()
               .Set(c => c.Tools, DToolbar.Tools)
               .Build(value => toolbar = value);

        builder.Field<KCheckBox>("ToolVisible").Value("True").ValueChanged(OnToolVisibleChanged).Set(f => f.Text, "编辑按钮可见").Build();
    }

    private void OnToolVisibleChanged(string value)
    {
        var check = Utils.ConvertTo<bool>(value);
        toolbar?.SetItemVisible(check, "Edit");
    }
}