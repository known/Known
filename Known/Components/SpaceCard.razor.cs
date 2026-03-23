namespace Known.Components;

/// <summary>
/// 个人空间卡片组件类。
/// </summary>
public partial class SpaceCard
{
    /// <summary>
    /// 取得或设置右侧内容模板。
    /// </summary>
    [Parameter] public RenderFragment Right { get; set; }

    /// <summary>
    /// 取得或设置统计数量信息列表。
    /// </summary>
    [Parameter] public List<StatisticCountInfo> Counts { get; set; }

    /// <summary>
    /// 取得或设置自定义问候语委托。
    /// </summary>
    [Parameter] public Func<UserInfo, string> OnGreeting { get; set; }

    /// <summary>
    /// 设置统计数量信息列表。
    /// </summary>
    /// <param name="counts"></param>
    public void SetCounts(List<StatisticCountInfo> counts)
    {
        Counts = counts;
        StateChanged();
    }

    //private string GetUserGreeting()
    //{
    //    var user = CurrentUser;
    //    var hour = DateTime.Now.Hour;
    //    var greet = GetGreeting(Language.Greeting0, user);
    //    if (5 <= hour && hour < 9) greet = GetGreeting(Language.Greeting5, user);
    //    else if (9 <= hour && hour < 11) greet = GetGreeting(Language.Greeting9, user);
    //    else if (11 <= hour && hour < 13) greet = GetGreeting(Language.Greeting11, user);
    //    else if (13 <= hour && hour < 15) greet = GetGreeting(Language.Greeting13, user);
    //    else if (15 <= hour && hour < 18) greet = GetGreeting(Language.Greeting15, user);
    //    else if (18 <= hour && hour < 22) greet = GetGreeting(Language.Greeting18, user);
    //    else if (22 <= hour && hour < 23) greet = GetGreeting(Language.Greeting22, user);
    //    else if (23 <= hour || hour < 5) greet = GetGreeting(Language.Greeting23, user);
    //    return greet;
    //}

    private string GetUserGreeting()
    {
        var user = CurrentUser;
        if (OnGreeting != null)
            return OnGreeting.Invoke(user);

        var currentHour = DateTime.Now.Hour;
        string greeting;

        if (currentHour is >= 5 and < 9)
            greeting = GetGreeting(Language.Greeting5, user);
        else if (currentHour is >= 9 and < 12)
            greeting = GetGreeting(Language.Greeting9, user);
        else if (currentHour is >= 12 and < 14)
            greeting = GetGreeting(Language.Greeting12, user);
        else if (currentHour is >= 14 and < 17)
            greeting = GetGreeting(Language.Greeting14, user);
        else if (currentHour is >= 17 and < 19)
            greeting = GetGreeting(Language.Greeting17, user);
        else if (currentHour is >= 19 and < 22)
            greeting = GetGreeting(Language.Greeting19, user);
        else if (currentHour is >= 22 and < 24)
            greeting = GetGreeting(Language.Greeting22, user);
        else
            greeting = GetGreeting(Language.Greeting24, user);

        return greeting;
    }

    private string GetGreeting(string id, UserInfo user) => Language[id].Replace("{name}", user?.Name);
}