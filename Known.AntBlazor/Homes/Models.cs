namespace Known.AntBlazor.Homes;

public class StatisticCountInfo
{
    public string Type { get; set; }
    public string Name { get; set; }
    public int? Count { get; set; }
    public string Url { get; set; }
}

public class ChartCardOption
{
    public string Id { get; set; }
    public string Title { get; set; }
    public List<CardChartInfo> Charts { get; set; } = [];
}

public class CardChartInfo
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public ChartDataInfo[] Datas { get; set; }
}