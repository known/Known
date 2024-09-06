namespace Known.AntBlazor.Components;

public class AntCol : ComponentBase
{
    [CascadingParameter] private AntRow Row { get; set; }

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
        var style = CssBuilder.Default().AddStyle(Style);
        if (Row != null && Row.Gutter > 0)
        {
            style.Add("padding-left", $"{Row.Gutter / 2}px");
            style.Add("padding-right", $"{Row.Gutter / 2}px");
        }
        builder.Div().Class(className).Style(style.BuildStyle())
               .Children(() => builder.Fragment(ChildContent))
               .Close();
    }
}