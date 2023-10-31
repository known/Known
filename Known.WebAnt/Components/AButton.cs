using Known.Razor;
using Microsoft.AspNetCore.Components.Web;

namespace Known.WebAnt.Components;

public class AButton : BaseComponent
{
    [Parameter] public KButton Button { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Button>(Button.Id)
               .Set(c => c.Type, ButtonType.Primary)
               .Set(c => c.Icon, Button.Icon)
               .Set(c => c.OnClick, Callback<MouseEventArgs>(OnButtonClick))
               .Set(c => c.ChildContent, BuildContent)
               .Build();
    }

    private void OnButtonClick(MouseEventArgs e)
    {
        Button.OnClick.InvokeAsync(e);
    }

    private void BuildContent(RenderTreeBuilder builder)
    {
        builder.Span(Button.Text);
    }
}