namespace Sample.WasmApi.Services;

public interface IHomeService : IService
{
    Task<HomeInfo> GetHomeAsync();
}

[WebApi]
class HomeService(Context context) : ServiceBase(context), IHomeService
{
    public async Task<HomeInfo> GetHomeAsync()
    {
        var user = CurrentUser;
        if (user == null)
            return new HomeInfo();

        var info = new HomeInfo
        {
            VisitMenuIds = [],
            Statistics = await GetStatisticsInfoAsync()
        };
        return info;
    }

    private Task<StatisticsInfo> GetStatisticsInfoAsync()
    {
        var info = new StatisticsInfo
        {
            UserCount = 100,
            LogCount = 54392
        };
        var now = DateTime.Now;
        var endDay = now.AddDays(1 - now.Day).AddMonths(1).AddDays(-1).Day;
        var begin = new DateTime(now.Year, now.Month, 1);
        var end = new DateTime(now.Year, now.Month, endDay, 23, 59, 59);
        var seriesLog = new Dictionary<string, object>();
        for (int i = 1; i <= endDay; i++)
        {
            seriesLog[i.ToString("00")] = new Random().Next(10, 200);
        }
        info.LogDatas =
        [
            new ChartDataInfo { Name = Language["Home.VisitCount"], Series = seriesLog }
        ];
        return Task.FromResult(info);
    }
}