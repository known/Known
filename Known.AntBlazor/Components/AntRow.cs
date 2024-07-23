namespace Known.AntBlazor.Components;

public class AntRow : ComponentBase
{
    [Parameter] public int Gutter { get; set; }
    [Parameter] public string Class { get; set; }
    [Parameter] public string Style { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var className = CssBuilder.Default("ant-row")
                          .AddClass(Class)
                          .BuildClass();
        var style = CssBuilder.Default().AddStyle(Style);
        if (Gutter > 0)
        {
            style.Add("margin-left", $"-{Gutter / 2}px");
            style.Add("margin-right", $"-{Gutter / 2}px");
        }
        builder.Div().Class(className).Style(style.BuildStyle()).Children(() =>
        {
            builder.Cascading<AntRow>(this, b => b.Fragment(ChildContent));
        }).Close();
    }
}