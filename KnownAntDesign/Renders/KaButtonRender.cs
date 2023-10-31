namespace KnownAntDesign.Renders;

class KaButtonRender : BaseRender<KButton>
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Button>(Component.Id)
               .Set(c => c.Type, ButtonType.Primary)
               .Set(c => c.Icon, Component.Icon)
               .Set(c => c.OnClick, Component.Callback<MouseEventArgs>(OnButtonClick))
               .Set(c => c.ChildContent, BuildContent)
               .Build();
    }

    private void OnButtonClick(MouseEventArgs e)
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

    private void BuildContent(RenderTreeBuilder builder)
    {
        builder.Span(Component.Text);
    }
}