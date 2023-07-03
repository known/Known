namespace Known.Razor.Components;

public class Steps : BaseComponent
{
    private MenuItem curItem;
    private readonly List<MenuItem> finishItems = new();

    [Parameter] public string Style { get; set; }
    [Parameter] public List<MenuItem> Items { get; set; }
    [Parameter] public Action<MenuItem> OnChanged { get; set; }
    [Parameter] public Action OnFinished { get; set; }
    [Parameter] public Action<RenderTreeBuilder, MenuItem> Body { get; set; }

    protected override void OnInitialized()
    {
        curItem = Items?.FirstOrDefault();
        base.OnInitialized();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Items == null || Items.Count == 0)
            return;

        if (Body == null)
        {
            BuildStepHead(builder, Items);
        }
        else
        {
            var css = CssBuilder.Default("steps").AddClass(Style).Build();
            builder.Div(css, attr =>
            {
                BuildStepHead(builder, Items);
                builder.Div("step-body", attr =>
                {
                    Body.Invoke(builder, curItem);
                    BuildStepButtons(builder, curItem);
                });
            });
        }
    }

    private void BuildStepHead(RenderTreeBuilder builder, List<MenuItem> items)
    {
        builder.Ul("step", attr =>
        {
            foreach (var item in items)
            {
                var css = CssBuilder.Default("")
                                    .AddClass("finish", finishItems.Contains(item) && curItem != item)
                                    .AddClass("active", curItem == item)
                                    .Build();
                builder.Li(css, attr =>
                {
                    builder.Span("item", attr => builder.IconName(item.Icon, item.Name));
                });
            }
        });
    }

    private void BuildStepButtons(RenderTreeBuilder builder, MenuItem item)
    {
        builder.Div("step-btns", attr =>
        {
            if (item != Items.First())
                builder.Button("上一步", Callback(e => OnPrev(item)));
            if (item != Items.Last())
                builder.Button("下一步", Callback(e => OnNext(item)));
            else
                builder.Button("完成", Callback(OnFinished));
        });
    }

    private void OnPrev(MenuItem item)
    {
        OnChanged?.Invoke(item);
        finishItems.Remove(item);
        var index = Items.IndexOf(item);
        curItem = Items[index - 1];
    }

    private void OnNext(MenuItem item)
    {
        OnChanged?.Invoke(item);
        finishItems.Add(item);
        var index = Items.IndexOf(item);
        curItem = Items[index + 1];
    }
}