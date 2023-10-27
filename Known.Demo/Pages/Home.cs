namespace Known.Demo.Pages;

class Home : WebPage
{
    private UserInfo? user;
    private HomeInfo? info;
    private List<KMenuItem>? visitMenus;
    private KChart? chart;

    protected override async Task InitPageAsync()
    {
        user = CurrentUser;
        info = await Client.Home.GetHomeAsync();
        visitMenus = Config.GetMenus(info?.VisitMenuIds);
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        ShowChart();
        return base.OnAfterRenderAsync(firstRender);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("ws", attr =>
        {
            BuildWorkSpace(builder);
            builder.Div("row ws-row", attr =>
            {
                BuildDataChart(builder);
                BuildVisitMenus(builder);
            });
        });
    }

    private void BuildWorkSpace(RenderTreeBuilder builder)
    {
        builder.Div("box ws-card", attr =>
        {
            builder.Div("welcome", attr =>
            {
                builder.Img(attr => attr.Class("ws-avatar").Src(user?.AvatarUrl));
                builder.Div("ws-info", attr =>
                {
                    builder.Span("ws-name", info?.Greeting);
                    builder.Span("ws-tips", attr => builder.Component<KTimer>().Build());
                });
            });

            builder.Ul("count", attr =>
            {
                BuildWDCount(builder, "用户数量", info?.Statistics?.UserCount);
                BuildWDCount(builder, "日志数量", info?.Statistics?.LogCount);
            });
        });
    }

    private void BuildDataChart(RenderTreeBuilder builder)
    {
        builder.Div("ws-chart", attr =>
        {
            builder.Component<KCard>()
                   .Set(c => c.Head, BuildDCHead)
                   .Set(c => c.Body, BuildDCBody)
                   .Build();
        });
    }

    private void BuildVisitMenus(RenderTreeBuilder builder)
    {
        builder.Div("ws-func", attr =>
        {
            builder.Component<KCard>()
                   .Set(c => c.Icon, "fa fa-th")
                   .Set(c => c.Name, "常用功能")
                   .Set(c => c.Body, BuildVMBody)
                   .Build();
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
        builder.Component<KChart>()
               .Set(c => c.Id, "chartData")
               .Build(value => chart = value);
    }

    private void ShowChart()
    {
        if (chart == null)
            return;

        var title = $"{DateTime.Now:yyyyMM}月系统访问量统计";
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