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
            VisitMenuIds = await Logger.GetVisitMenuIdsAsync(db, user.UserName, 12),
            Statistics = await GetStatisticsInfoAsync(db)
        };
        await db.CloseAsync();
        return info;
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
        var seriesLog = new Dictionary<string, object>();
        for (int i = 1; i <= endDay; i++)
        {
            var date = new DateTime(now.Year, now.Month, i);
            seriesLog[i.ToString("00")] = await GetLogCountAsync(db, date);
        }
        info.LogDatas =
        [
            new ChartDataInfo { Name = Language["Home.VisitCount"], Series = seriesLog }
        ];
        return info;
    }

    private static Task<int> GetLogCountAsync(Database db, DateTime date)
    {
        var day = date.ToString("yyyy-MM-dd");
        var begin = DateTime.Parse($"{day} 00:00:00");
        var end = DateTime.Parse($"{day} 23:59:59");
        return db.Query<SysLog>()
                 .Where(d => d.CompNo == db.User.CompNo && d.CreateTime.Between(begin, end))
                 .CountAsync();
    }
}