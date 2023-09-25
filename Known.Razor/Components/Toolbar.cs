namespace Known.Razor.Components;

public class Toolbar : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public List<ButtonInfo> Tools { get; set; }
    [Parameter] public Action<ButtonInfo> OnAction { get; set; }

    public void SetItemName(string id, string name)
    {
        var item = Tools?.FirstOrDefault(t => t.Id == id);
        if (item != null)
        {
            item.Name = name;
            StateChanged();
        }
    }

    public void SetItemEnabled(bool enabled, params string[] itemIds)
    {
        foreach (var id in itemIds)
        {
            var item = Tools?.FirstOrDefault(t => t.Id == id);
            if (item != null)
                item.Enabled = enabled;
        }
        StateChanged();
    }

    public void SetItemVisible(bool visible, params string[] itemIds)
    {
        foreach (var id in itemIds)
        {
            var item = Tools?.FirstOrDefault(t => t.Id == id);
            if (item != null)
                item.Visible = visible;
        }
        StateChanged();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Visible) return;
        if (Tools == null || Tools.Count == 0) return;

        var css = CssBuilder.Default("toolbar").AddClass(Style).Build();
        builder.Div(css, attr =>
        {
            foreach (var item in Tools)
            {
                if (item == null || !item.Visible) continue;

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