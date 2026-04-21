namespace Known.Server.Pages;

public partial class Index
{
    private IHomeService Service;
    private SpaceCard space;
    private ChartCard chart;
    private CommFuncCard func;

    public override RenderFragment GetPageTitle()
    {
        return GetPageTitle("home", Language.Home);
    }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<IHomeService>();
    }

    protected override async Task OnRenderAsync(bool firstRender)
    {
        await base.OnRenderAsync(firstRender);
        if (firstRender)
        {
            var info = await Service.GetHomeAsync();
            var counts = new List<StatisticCountInfo>
            {
                new() { Name = Language.HomeUserCount, Count = info?.Statistics?.UserCount },
                new() { Name = Language.HomeLogCount, Count = info?.Statistics?.LogCount }
            };
            space?.SetCounts(counts);

            var option = new ChartCardOption { Id = "Order", Title = Language.HomeLogStatistic };
            option.Charts.Add(new CardChartInfo
            {
                Type = "Bar",
                Title = Language[Language.HomeVisitTitle].Replace("{month}", $"{DateTime.Now:yyyyMM}"),
                Datas = info?.Statistics?.LogDatas
            });
            await chart?.SetOptionAsync(option);
            func?.SetMenus(info?.VisitMenuIds);
        }
    }
}