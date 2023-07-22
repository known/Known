namespace Known.Razor.Components;

public class Toolbar : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public List<ButtonInfo> Tools { get; set; }
    [Parameter] public Action<ButtonInfo> OnAction { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default("toolbar").AddClass(Style).Build();
        builder.Div(css, attr =>
        {
            foreach (var item in Tools)
            {
                if (item.Children.Any())
                {
                    var items = item.Children.Select(i => new MenuItem(i, () => OnToolAction(i))).ToList();
                    builder.Dropdown(items, item.Name, "button");
                }
                else
                {
                    BuildButton(builder, item);
                }
            }
        });
    }

    private void BuildButton(RenderTreeBuilder builder, ButtonInfo item)
    {
        builder.Button(item, Callback(() => OnToolAction(item)));
    }

    private void OnToolAction(ButtonInfo item) => OnAction?.Invoke(item);
}