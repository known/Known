namespace Known.Test.Services;

class HomeService : BaseService
{
    internal HomeService(Context context) : base(context) { }

    internal HomeInfo GetHome()
    {
        var user = CurrentUser;
        return new HomeInfo
        {
            Greeting = GetUserGreeting(),
            VisitMenuIds = Logger.GetVisitMenuIds(Database, user.UserName, 12),
            Statistics = GetStatisticsInfo()
        };
    }

    private string GetUserGreeting()
    {
        var user = CurrentUser;
        var hour = DateTime.Now.Hour;
        var greet = $"您好！{user?.Name}，您貌似不在我们的时空中！";
        if (5 <= hour && hour < 9)
            greet = $"早安！{user?.Name}，开始您一天的工作吧！";
        else if (9 <= hour && hour < 11)
            greet = $"上午好！{user?.Name}，加油！";
        else if (11 <= hour && hour < 13)
            greet = $"午安！{user?.Name}，别忘了准备午饭！";
        else if (13 <= hour && hour < 15)
            greet = $"下午好！{user?.Name}，继续加油！";
        else if (15 <= hour && hour < 18)
            greet = $"下午好！{user?.Name}，工作一天累了吧，泡杯下午茶解解乏！";
        else if (18 <= hour && hour < 22)
            greet = $"晚上好！{user?.Name}，晚饭吃了吗？";
        else if (22 <= hour && hour < 23)
            greet = $"晚安！{user?.Name}，该睡觉了，明天还会天亮的！";
        else if (23 <= hour || hour < 5)
            greet = $"{user?.Name}，还没休息啊，身体是革命的本钱！";

        return greet;
    }

    private StatisticsInfo GetStatisticsInfo()
    {
        Database.Open();
        var info = new StatisticsInfo
        {
            UserCount = Database.Scalar<int>("select count(*) from SysUser"),
            LogCount = Database.Scalar<int>("select count(*) from SysLog")
        };
        var now = DateTime.Now;
        var endDay = now.AddDays(1 - now.Day).AddMonths(1).AddDays(-1).Day;
        var seriesLog = new Dictionary<string, object>();
        for (int i = 1; i <= endDay; i++)
        {
            var date = new DateTime(now.Year, now.Month, i);
            seriesLog[i.ToString("00")] = GetLogCount(Database, date);
        }
        info.LogDatas = new ChartDataInfo[]
        {
            new ChartDataInfo { Name = "访问量", Series = seriesLog }
        };
        Database.Close();
        return info;
    }

    private static object GetLogCount(Database db, DateTime date)
    {
        var day = date.ToString("yyyy-MM-dd");
        var sql = $@"select count(1) from SysLog where CreateTime between '{day} 00:00:00' and '{day} 23:59:59'";
        if (db.DatabaseType == DatabaseType.Access)
            sql = sql.Replace("'", "#");
        return db.Scalar<int>(sql);
    }
}