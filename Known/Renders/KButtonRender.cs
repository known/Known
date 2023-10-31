namespace Known.Renders;

class KButtonRender : BaseRender<KButton>
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default(Component.Type.ToString().ToLower())
                            .AddClass(Component.Style)
                            .Build();
        builder.Button(css, attr =>
        {
            attr.Id(Component.Id).Disabled(!Component.Enabled).OnClick(Component.Callback(OnButtonClick));
            builder.IconName(Component.Icon, Component.Text);
        });
    }

    private void OnButtonClick()
    {
        UI.Enabled(Component.Id, false);
        if (Component.OnClick.HasDelegate)
        {
            var task = Component.OnClick.InvokeAsync();
            if (task.IsCompleted)
            {
                UI.Enabled(Component.Id, true);
            }
        }
    }
}