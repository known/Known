using Known.Shared.Models;
using Known.Shared.Repositories;

namespace Known.Shared.Services;

class HomeService : ServiceBase
{
    public async Task<HomeInfo> GetHomeAsync()
    {
        var user = CurrentUser;
        return new HomeInfo
        {
            Greeting = GetUserGreeting(),
            VisitMenuIds = await Logger.GetVisitMenuIdsAsync(Database, user.UserName, 12),
            Statistics = await GetStatisticsInfoAsync()
        };
    }

    private string GetUserGreeting()
    {
        var user = CurrentUser;
        var hour = DateTime.Now.Hour;
        var greet = Language["Greeting0"].Replace("{name}", user?.Name);
        if (5 <= hour && hour < 9)
            greet = Language["Greeting5"].Replace("{name}", user?.Name);
        else if (9 <= hour && hour < 11)
            greet = Language["Greeting9"].Replace("{name}", user?.Name);
        else if (11 <= hour && hour < 13)
            greet = Language["Greeting11"].Replace("{name}", user?.Name);
        else if (13 <= hour && hour < 15)
            greet = Language["Greeting13"].Replace("{name}", user?.Name);
        else if (15 <= hour && hour < 18)
            greet = Language["Greeting15"].Replace("{name}", user?.Name);
        else if (18 <= hour && hour < 22)
            greet = Language["Greeting18"].Replace("{name}", user?.Name);
        else if (22 <= hour && hour < 23)
            greet = Language["Greeting22"].Replace("{name}", user?.Name);
        else if (23 <= hour || hour < 5)
            greet = Language["Greeting23"].Replace("{name}", user?.Name);

        return greet;
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