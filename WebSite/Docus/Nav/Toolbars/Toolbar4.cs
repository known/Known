namespace WebSite.Docus.Nav.Toolbars;

[Title("按钮可用示例")]
class Toolbar4 : BaseComponent
{
    private Toolbar? toolbar;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Toolbar>()
               .Set(c => c.Tools, KToolbar.Tools)
               .Build(value => toolbar = value);

        builder.Field<CheckBox>("ToolEnabled").Value("True").ValueChanged(OnToolEnabledChanged).Set(f => f.Text, "验证、保存按钮可用").Build();
    }

    private void OnToolEnabledChanged(string value)
    {
        var check = Utils.ConvertTo<bool>(value);
        toolbar?.SetItemEnabled(check, "Check", "Save");
    }
}