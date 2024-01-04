namespace Known.Shared.Models;

class HomeInfo
{
    public List<string> VisitMenuIds { get; set; }
    public StatisticsInfo Statistics { get; set; }
}

class StatisticsInfo
{
    public int UserCount { get; set; }
    public int LogCount { get; set; }
    public ChartDataInfo[] LogDatas { get; set; }
}