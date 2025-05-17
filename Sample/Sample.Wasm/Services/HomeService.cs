namespace Sample.Wasm.Services;

public interface IHomeService : IService
{
    Task<HomeInfo> GetHomeAsync();
}

[Service]
class HomeService(Context context) : ServiceBase(context), IHomeService
{
    public async Task<HomeInfo> GetHomeAsync()
    {
        return new HomeInfo
        {
            VisitMenuIds = [],
            Statistics = await GetStatisticsInfoAsync()
        };
    }

    private Task<StatisticsInfo> GetStatisticsInfoAsync()
    {
        var info = new StatisticsInfo
        {
            UserCount = 100,
            LogCount = 54306
        };
        var now = DateTime.Now;
        var endDay = now.AddDays(1 - now.Day).AddMonths(1).AddDays(-1).Day;
        var begin = new DateTime(now.Year, now.Month, 1);
        var end = new DateTime(now.Year, now.Month, endDay, 23, 59, 59);
        var seriesLog = new Dictionary<string, object>();
        for (int i = 1; i <= endDay; i++)
        {
            var date = new DateTime(now.Year, now.Month, i).ToString("yyyy-MM-dd");
            seriesLog[i.ToString("00")] = new Random().Next(10, 200);
        }
        info.LogDatas =
        [
            new ChartDataInfo { Name = Language.HomeVisitCount, Series = seriesLog }
        ];
        return Task.FromResult(info);
    }
}