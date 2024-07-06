namespace Known.Blazor;

public class KIcon : BaseComponent
{
    [Parameter] public string Icon { get; set; }
    [Parameter] public EventCallback<MouseEventArgs>? OnClick { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Icon.StartsWith("fa"))
        {
            builder.Span(Icon, "", OnClick);
            return;
        }

        UI.BuildIcon(builder, Icon, OnClick);
    }
}