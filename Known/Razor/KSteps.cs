namespace Known.Razor;

public class KSteps : BaseComponent
{
    private KMenuItem curItem;
    private readonly List<KMenuItem> finishItems = new();

    [Parameter] public string Style { get; set; }
    [Parameter] public List<KMenuItem> Items { get; set; }
    [Parameter] public Action<KMenuItem> OnChanged { get; set; }
    [Parameter] public Action OnFinished { get; set; }
    [Parameter] public Action<RenderTreeBuilder, KMenuItem> Body { get; set; }

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

    private void BuildStepHead(RenderTreeBuilder builder, List<KMenuItem> items)
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

    private void BuildStepButtons(RenderTreeBuilder builder, KMenuItem item)
    {
        builder.Div("step-btns", attr =>
        {
            if (item != Items.First())
                builder.Button("上一步", Callback(e => OnPrev(item)), StyleType.Info);
            if (item != Items.Last())
                builder.Button("下一步", Callback(e => OnNext(item)), StyleType.Info);
            else
                builder.Button("完成", Callback(OnFinished), StyleType.Primary);
        });
    }

    private void OnPrev(KMenuItem item)
    {
        OnChanged?.Invoke(item);
        finishItems.Remove(item);
        var index = Items.IndexOf(item);
        curItem = Items[index - 1];
    }

    private void OnNext(KMenuItem item)
    {
        OnChanged?.Invoke(item);
        finishItems.Add(item);
        var index = Items.IndexOf(item);
        curItem = Items[index + 1];
    }
}