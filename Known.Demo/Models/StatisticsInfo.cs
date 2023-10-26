namespace Known.Demo.Models;

public class StatisticsInfo
{
    public int UserCount { get; set; }
    public int LogCount { get; set; }
    public ChartDataInfo[]? LogDatas { get; set; }
}