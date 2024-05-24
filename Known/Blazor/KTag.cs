namespace Known.Blazor;

public class KTag : BaseComponent
{
    [Parameter] public string Text { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var text = Language?.GetCode(Text);
        if (string.IsNullOrWhiteSpace(text))
            text = Text;
        UI.BuildTag(builder, text);
    }
}