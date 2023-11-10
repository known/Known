using AntDesign.Charts;
using Known.Extensions;
using Known.Razor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Web.Models;

class HomeInfo
{
    public string Greeting { get; set; }
    public List<string> VisitMenuIds { get; set; }
    public StatisticsInfo Statistics { get; set; }
}

class StatisticsInfo
{
    public int UserCount { get; set; }
    public int LogCount { get; set; }
    public ChartDataInfo[] LogDatas { get; set; }
}

class HomeService : BaseService
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

    private async Task<StatisticsInfo> GetStatisticsInfoAsync()
    {
        var user = CurrentUser;
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
        info.LogDatas = new ChartDataInfo[]
        {
            new ChartDataInfo { Name = "访问量", Series = seriesLog }
        };
        await Database.CloseAsync();
        return info;
    }
}

class HomeRepository
{
    internal static Task<int> GetUserCountAsync(Database db)
    {
        return db.ScalarAsync<int>("select count(*) from SysUser where CompNo=@CompNo", new { db.User.CompNo });
    }

    internal static Task<int> GetLogCountAsync(Database db)
    {
        return db.ScalarAsync<int>("select count(*) from SysLog where CompNo=@CompNo", new { db.User.CompNo });
    }

    internal static Task<int> GetLogCountAsync(Database db, DateTime date)
    {
        var day = date.ToString("yyyy-MM-dd");
        var sql = $@"select count(1) from SysLog where CompNo=@CompNo and CreateTime between '{day} 00:00:00' and '{day} 23:59:59'";
        if (db.DatabaseType == DatabaseType.Access)
            sql = sql.Replace("'", "#");
        return db.ScalarAsync<int>(sql, new { db.User.CompNo });
    }
}

class HomeLogChart : BaseComponent
{
    private ColumnConfig config = new()
    {
        Padding = "auto",
        XField = "Day",
        YField = "Count",
        Meta = new
        {
            Type = new { Alias = "日期" },
            Sales = new { Alias = "数量" }
        }
    };

    [Parameter] public ChartDataInfo[] Datas { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var data = Datas.FirstOrDefault().Series.Select(d => new { Month = d.Key, Count = d.Value }).ToArray();
        builder.Component<AntDesign.Charts.Column>()
               .Set(c => c.Config, config)
               .Set(c => c.Data, data)
               .Build();
    }
}