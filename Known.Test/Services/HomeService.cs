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
            VisitMenuIds = LogService.GetVisitMenuIds(Database, user.UserId, 12),
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
        var info = new StatisticsInfo
        {
        };
        return info;
    }
}