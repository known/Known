namespace Known.AntBlazor.Components;

public class AntCol : ComponentBase
{
    [Parameter] public int Span { get; set; }
    [Parameter] public int Offset { get; set; }
    [Parameter] public string Class { get; set; }
    [Parameter] public string Style { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var className = CssBuilder.Default("ant-col")
                          .AddClass($"ant-col-{Span}", Span > 0)
                          .AddClass($"ant-col-offset-{Offset}", Offset > 0)
                          .AddClass(Class)
                          .BuildClass();
        builder.Div().Class(className).Style(Style).Children(() => builder.Fragment(ChildContent)).Close();
    }
}