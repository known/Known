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
    /// 设置统计数量信息列表。
    /// </summary>
    /// <param name="counts"></param>
    public void SetCounts(List<StatisticCountInfo> counts)
    {
        Counts = counts;
        StateChanged();
    }

    private string GetUserGreeting()
    {
        var user = CurrentUser;
        var hour = DateTime.Now.Hour;
        var greet = GetGreeting(Language.Greeting0, user);
        if (5 <= hour && hour < 9) greet = GetGreeting(Language.Greeting5, user);
        else if (9 <= hour && hour < 11) greet = GetGreeting(Language.Greeting9, user);
        else if (11 <= hour && hour < 13) greet = GetGreeting(Language.Greeting11, user);
        else if (13 <= hour && hour < 15) greet = GetGreeting(Language.Greeting13, user);
        else if (15 <= hour && hour < 18) greet = GetGreeting(Language.Greeting15, user);
        else if (18 <= hour && hour < 22) greet = GetGreeting(Language.Greeting18, user);
        else if (22 <= hour && hour < 23) greet = GetGreeting(Language.Greeting22, user);
        else if (23 <= hour || hour < 5) greet = GetGreeting(Language.Greeting23, user);
        return greet;
    }

    private string GetGreeting(string id, UserInfo user) => Language[id].Replace("{name}", user?.Name);
}