namespace Known.Test.Pages.Samples;

class DevSample : PageComponent
{
    private readonly List<MenuItem> items = new()
    {
        new MenuItem("列表示例", "fa fa-list", typeof(DemoDataList)),
        new MenuItem("表单示例", "fa fa-th-list", typeof(DemoForm)),
        new MenuItem("弹窗示例", "fa fa-window-maximize", typeof(DemoDialog)),
        new MenuItem("图表示例", "fa fa-bar-chart", typeof(DemoChart)),
        new MenuItem("其他示例", "fa fa-file-o", typeof(DemoOther))
    };
    private MenuItem curItem;

    protected override void OnInitialized()
    {
        curItem = items[0];
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Div("tabs box demo", attr =>
        {
            builder.Component<Tab>()
               .Set(c => c.Position, "left")
               .Set(c => c.CurItem, curItem?.Id)
               .Set(c => c.Items, items)
               .Set(c => c.OnChanged, OnTabChanged)
               .Build();
            builder.Div("tab-body left content", attr => builder.DynamicComponent(curItem?.ComType));
        });
    }

    private void OnTabChanged(MenuItem item)
    {
        curItem = item;
        StateChanged();
    }
}