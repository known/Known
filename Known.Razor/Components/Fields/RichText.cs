namespace Known.Razor.Components.Fields;

public class RichText : Field
{
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            UI.InitEditor(Id);
        return base.OnAfterRenderAsync(firstRender);
    }

    protected override void BuildText(RenderTreeBuilder builder)
    {
        builder.Markup(Value);
    }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        builder.Div("editor", attr => attr.Id(Id));
    }
}