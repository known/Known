namespace Known.Demo.Models;

class StatisticsInfo
{
    public int UserCount { get; set; }
    public int LogCount { get; set; }
    public ChartDataInfo[] LogDatas { get; set; }
}