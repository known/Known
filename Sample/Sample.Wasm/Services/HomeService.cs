using Sample.Models;

namespace Sample.Services;

public interface IHomeService : IService
{
    Task<PagingResult<HomeInfo>> QueryHomesAsync(PagingCriteria criteria);
    Task<HomeInfo> GetHomeAsync();
}

[Client]
class HomeClient(HttpClient http) : ClientBase(http), IHomeService
{
    public Task<PagingResult<HomeInfo>> QueryHomesAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<HomeInfo>("/Home/QueryHomes", criteria);
    }

    public Task<HomeInfo> GetHomeAsync()
    {
        return Http.GetAsync<HomeInfo>("/Home/GetHome");
    }
}

[WebApi, Service]
class HomeService(Context context) : ServiceBase(context), IHomeService
{
    public Task<PagingResult<HomeInfo>> QueryHomesAsync(PagingCriteria criteria)
    {
        return Task.FromResult(new PagingResult<HomeInfo>());
    }

    public async Task<HomeInfo> GetHomeAsync()
    {
        var info = new HomeInfo();
        var user = CurrentUser;
        if (user == null)
            return info;

        await Database.QueryActionAsync(async db =>
        {
            info.VisitMenuIds = await db.GetVisitMenuIdsAsync(user.UserName, 12);
            info.Statistics = await GetStatisticsInfoAsync(db);
        });
        return info;
    }

    private static async Task<StatisticsInfo> GetStatisticsInfoAsync(Database db)
    {
        var info = new StatisticsInfo
        {
            //UserCount = await db.CountAsync<SysUser>(d => d.CompNo == db.User.CompNo),
            LogCount = await db.CountAsync<SysLog>(d => d.CompNo == db.User.CompNo)
        };
        var now = DateTime.Now;
        var endDay = now.AddDays(1 - now.Day).AddMonths(1).AddDays(-1).Day;
        var begin = new DateTime(now.Year, now.Month, 1);
        var end = new DateTime(now.Year, now.Month, endDay, 23, 59, 59);
        var logs = await db.QueryListAsync<SysLog>(d => d.CompNo == db.User.CompNo && d.CreateTime >= begin && d.CreateTime <= end);
        var seriesLog = new Dictionary<string, object>();
        for (int i = 1; i <= endDay; i++)
        {
            var date = new DateTime(now.Year, now.Month, i).ToString("yyyy-MM-dd");
            seriesLog[i.ToString("00")] = logs?.Count(l => l.CreateTime.ToString("yyyy-MM-dd") == date);
        }
        info.LogDatas =
        [
            new ChartDataInfo { Name = Language.HomeVisitCount, Series = seriesLog }
        ];
        return info;
    }
}