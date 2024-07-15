namespace Sample.Web.Services;

class HomeService(Context context) : ServiceBase(context), IHomeService
{
    public async Task<HomeInfo> GetHomeAsync()
    {
        var user = CurrentUser;
        if (user == null)
            return new HomeInfo();

        return new HomeInfo
        {
            VisitMenuIds = await Logger.GetVisitMenuIdsAsync(Database, user.UserName, 12),
            Statistics = await GetStatisticsInfoAsync()
        };
    }

    private async Task<StatisticsInfo> GetStatisticsInfoAsync()
    {
        await Database.OpenAsync();
        var info = new StatisticsInfo
        {
            UserCount = await HomeRepository.GetUserCountAsync(Database),
            LogCount = await HomeRepository.GetLogCountAsync(Database)
        };
        var now = DateTime.Now;
        var endDay = now.AddDays(1 - now.Day).AddMonths(1).AddDays(-1).Day;
        var seriesLog = new Dictionary<string, object>();
        for (int i = 1; i <= endDay; i++)
        {
            var date = new DateTime(now.Year, now.Month, i);
            seriesLog[i.ToString("00")] = await HomeRepository.GetLogCountAsync(Database, date);
        }
        info.LogDatas =
        [
            new ChartDataInfo { Name = Language["Home.VisitCount"], Series = seriesLog }
        ];
        await Database.CloseAsync();
        return info;
    }
}