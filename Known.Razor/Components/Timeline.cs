namespace Known.Razor.Components;

public class Timeline<T> : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public List<T> Items { get; set; }
    [Parameter] public Action<RenderTreeBuilder, T> Template { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default("timeline").AddClass(Style).Build();
        builder.Ul(css, attr =>
        {
            if (Items != null && Items.Count > 0)
            {
                foreach (var item in Items)
                {
                    builder.Li(attr => Template?.Invoke(builder, item));
                }
            }
        });
    }
}