namespace Known.Test.Pages;

class Home : PageComponent
{
    private UserInfo user;
    private HomeInfo info;
    private List<MenuItem> visitMenus;
    private Chart chart;

    protected override void OnInitialized()
    {
        var service = new HomeService(Context);
        user = CurrentUser;
        info = service.GetHome();
        visitMenus = KRConfig.GetMenus(info?.VisitMenuIds);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildWorkSpace(builder);
        builder.Div("ws-row", attr =>
        {
            BuildDataChart(builder);
            BuildVisitMenus(builder);
        });
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
            ShowChart();
    }

    private void BuildWorkSpace(RenderTreeBuilder builder)
    {
        builder.Component<Card>()
               .Set(c => c.Style, "ws-card")
               .Set(c => c.Body, BuildWSCard)
               .Build();
    }

    private void BuildDataChart(RenderTreeBuilder builder)
    {
        builder.Component<Card>()
               .Set(c => c.Style, "ws-chart")
               .Set(c => c.Head, BuildDCHead)
               .Set(c => c.Body, BuildDCBody)
               .Build();
    }

    private void BuildVisitMenus(RenderTreeBuilder builder)
    {
        builder.Component<Card>()
               .Set(c => c.Style, "ws-func")
               .Set(c => c.Icon, "fa fa-th")
               .Set(c => c.Name, "常用功能")
               .Set(c => c.Body, BuildVMBody)
               .Build();
    }

    private void BuildWSCard(RenderTreeBuilder builder)
    {
        builder.Div("ws-title", "工作台");
        builder.Img(attr => attr.Class("ws-avatar").Src($"_content/Known.Razor{user?.AvatarUrl}"));
        builder.Span("ws-name", info?.Greeting);
        builder.Span("ws-tips", $"今天是：{DateTime.Now:yyyy-MM-dd dddd}");

        builder.Ul("count", attr =>
        {
            BuildWDCount(builder, "用户数量", info?.Statistics?.UserCount);
            BuildWDCount(builder, "日志数量", info?.Statistics?.LogCount);
        });
    }

    private static void BuildWDCount(RenderTreeBuilder builder, string name, decimal? count)
    {
        builder.Li(attr =>
        {
            builder.Span("name", attr =>
            {
                builder.Text(name);
                builder.Span("month", "总");
            });
            builder.Span("amount", $"{count}");
        });
    }

    private void BuildDCHead(RenderTreeBuilder builder)
    {
        builder.IconName("fa fa-bar-chart", "数据统计");
    }

    private void BuildDCBody(RenderTreeBuilder builder)
    {
        builder.Component<Chart>()
               .Set(c => c.Id, "chartData")
               .Build(value => chart = value);
    }

    private void ShowChart()
    {
        if (chart == null)
            return;

        var title = $"{DateTime.Now:yyyy年MM月}系统访问量统计";
        chart.YAxis = new { title = new { text = "数量" } };
        chart.ShowBar(title, info?.Statistics?.LogDatas);
    }

    private void BuildVMBody(RenderTreeBuilder builder)
    {
        if (visitMenus == null || visitMenus.Count == 0)
            return;

        foreach (var item in visitMenus)
        {
            builder.Div("ws-func-menu", attr =>
            {
                attr.OnClick(Callback(() => Context.Navigate(item)));
                builder.IconName(item.Icon, item.Name);
            });
        }
    }
}