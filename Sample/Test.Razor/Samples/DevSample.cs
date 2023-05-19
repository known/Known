namespace Test.Razor.Samples;

class DevSample : PageComponent
{
    private readonly List<MenuItem> items = new()
    {
        new MenuItem("表单组件", "fa fa-th-list", typeof(DemoForm)),
        new MenuItem("表格组件", "fa fa-table", typeof(DemoDataGrid)),
        new MenuItem("列表组件", "fa fa-list", typeof(DemoDataList)),
        new MenuItem("弹窗组件", "fa fa-window-maximize", typeof(DemoDialog)),
        new MenuItem("图表组件", "fa fa-bar-chart", typeof(DemoChart)),
        new MenuItem("其他组件", "fa fa-file-o", typeof(DemoOther))
    };
    private MenuItem? curItem;

    protected override void OnInitialized()
    {
        curItem = items[0];
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Div("tabs demo", attr =>
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