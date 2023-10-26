namespace WebSite.Docus.Nav.Toolbars;

class Toolbar4 : BaseComponent
{
    private KToolbar? toolbar;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KToolbar>()
               .Set(c => c.Tools, DToolbar.Tools)
               .Build(value => toolbar = value);

        builder.Field<KCheckBox>("ToolEnabled").Value("True").ValueChanged(OnToolEnabledChanged).Set(f => f.Text, "验证、保存按钮可用").Build();
    }

    private void OnToolEnabledChanged(string value)
    {
        var check = Utils.ConvertTo<bool>(value);
        toolbar?.SetItemEnabled(check, "Check", "Save");
    }
}