namespace Known.Razor.Components;

public class Tab : BaseComponent
{
    [Parameter] public bool Justified { get; set; }
    [Parameter] public string Position { get; set; } = "top";
    [Parameter] public string Codes { get; set; }
    [Parameter] public string CurItem { get; set; }
    [Parameter] public List<MenuItem> Items { get; set; }
    [Parameter] public Action<MenuItem> OnChanged { get; set; }

    private List<MenuItem> TabItems { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (Items != null && Items.Count > 0)
            TabItems = Items;
        else
            TabItems = CodeInfo.GetCodes(Codes).Select(c => new MenuItem(c.Code, c.Name)).ToList();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Ul($"tab {Position}", attr =>
        {
            var items = TabItems;
            if (items != null && items.Count != 0)
            {
                foreach (var item in items)
                {
                    builder.Li(Active(item.Code), attr =>
                    {
                        attr.OnClick(Callback(e => OnItemClick(item)));
                        if (Justified)
                        {
                            var width = Math.Round(100M / items.Count, 2);
                            attr.Style($"width:{width}%");
                        }
                        if (!string.IsNullOrWhiteSpace(item.Icon))
                            builder.Icon(item.Icon);
                        builder.Span(item.Name);
                    });
                }
            }
        });
    }

    private void OnItemClick(MenuItem item)
    {
        CurItem = item.Code;
        OnChanged?.Invoke(item);
    }

    private string Active(string item) => CurItem == item ? "active" : "";
}