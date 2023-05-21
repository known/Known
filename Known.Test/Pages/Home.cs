namespace Known.Test.Pages;

class Home : PageComponent
{
    private UserInfo user;
    private HomeInfo info;
    private List<MenuItem> visitMenus;
    private Chart chart;
    private int curChartType = 1;
    private string IsChartActive(int type) => curChartType == type ? "active" : "";

    protected override Task InitPageAsync()
    {
        var service = new HomeService(Context);
        user = CurrentUser;
        info = service.GetHome();
        visitMenus = KRConfig.GetMenus(info?.VisitMenuIds);
        OnChartClick(1);
        return base.InitPageAsync();
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

    private void BuildWorkSpace(RenderTreeBuilder builder)
    {
        builder.Component<Card>()
               .Set(c => c.Style, "ws-card")
               .Set(c => c.Body, BuildTree(BuildWSCard))
               .Build();
    }

    private void BuildDataChart(RenderTreeBuilder builder)
    {
        builder.Component<Card>()
               .Set(c => c.Style, "ws-chart")
               .Set(c => c.Head, BuildTree(BuildDCHead))
               .Set(c => c.Body, BuildTree(BuildDCBody))
               .Build();
    }

    private void BuildVisitMenus(RenderTreeBuilder builder)
    {
        builder.Component<Card>()
               .Set(c => c.Style, "ws-func")
               .Set(c => c.Icon, "fa fa-th")
               .Set(c => c.Name, "常用功能")
               .Set(c => c.Body, BuildTree(BuildVMBody))
               .Build();
    }

    private void BuildWSCard(RenderTreeBuilder builder)
    {
        builder.Div("ws-title", "工作台");
        builder.Img(attr => attr.Class("ws-avatar").Src($"_content/Known.Razor{user?.AvatarUrl}"));
        builder.Span("ws-name", info?.Greeting);
        builder.Span("ws-tips", $"今天是：{DateTime.Now:yyyy-MM-dd dddd}");
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

    private void OnChartClick(int type)
    {
        if (chart == null)
            return;

        curChartType = type;
        if (type == 1)
            chart.YAxis = new { title = new { text = "单量" } };
        else if (type == 2)
            chart.YAxis = new { title = new { text = "金额" } };
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