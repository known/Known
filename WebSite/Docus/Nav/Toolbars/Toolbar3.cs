namespace WebSite.Docus.Nav.Toolbars;

[Title("按钮可见示例")]
class Toolbar3 : BaseComponent
{
    private Toolbar? toolbar;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Toolbar>()
               .Set(c => c.Tools, KToolbar.Tools)
               .Build(value => toolbar = value);

        builder.Field<CheckBox>("ToolVisible").Value("True").ValueChanged(OnToolVisibleChanged).Set(f => f.Text, "编辑按钮可见").Build();
    }

    private void OnToolVisibleChanged(string value)
    {
        var check = Utils.ConvertTo<bool>(value);
        toolbar?.SetItemVisible(check, "Edit");
    }
}