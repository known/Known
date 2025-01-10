namespace Sample.Photino.Models;

public class HomeInfo
{
    public List<string> VisitMenuIds { get; set; }
    public StatisticsInfo Statistics { get; set; }
}

public class StatisticsInfo
{
    public int UserCount { get; set; }
    public int LogCount { get; set; }
    public ChartDataInfo[] LogDatas { get; set; }
}