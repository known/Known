namespace Sample.Web.Services;

class HomeService(Context context) : ServiceBase(context), IHomeService
{
    public async Task<HomeInfo> GetHomeAsync()
    {
        var user = CurrentUser;
        if (user == null)
            return new HomeInfo();

        var db = Database;
        await db.OpenAsync();
        var info = new HomeInfo
        {
            VisitMenuIds = await GetVisitMenuIdsAsync(db, user.UserName, 12),
            Statistics = await GetStatisticsInfoAsync(db)
        };
        await db.CloseAsync();
        return info;
    }

    private async Task<List<string>> GetVisitMenuIdsAsync(Database db, string userName, int size)
    {
        var logs = await db.Query<SysLog>()
                           .Where(d => d.Type == $"{LogType.Page}" && d.CreateBy == userName)
                           .GroupBy(d => d.Target)
                           .Select(d => new CountInfo { Field1 = d.Target, TotalCount = DbFunc.Count() })
                           .ToListAsync();
        logs = logs?.OrderByDescending(f => f.TotalCount).Take(size).ToList();
        return logs?.Select(l => l.Field1).ToList();
    }

    private async Task<StatisticsInfo> GetStatisticsInfoAsync(Database db)
    {
        var info = new StatisticsInfo
        {
            UserCount = await db.CountAsync<SysUser>(d => d.CompNo == db.User.CompNo),
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
            new ChartDataInfo { Name = Language["Home.VisitCount"], Series = seriesLog }
        ];
        return info;
    }
}