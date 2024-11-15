namespace Known.Components;

/// <summary>
/// 统计数量信息类。
/// </summary>
public class StatisticCountInfo
{
    /// <summary>
    /// 取得或设置统计类型（总、年、月等）。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置统计名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置统计数量。
    /// </summary>
    public int? Count { get; set; }

    /// <summary>
    /// 取得或设置点击数量导航的地址。
    /// </summary>
    public string Url { get; set; }
}

/// <summary>
/// 图表卡片选项类。
/// </summary>
public class ChartCardOption
{
    /// <summary>
    /// 取得或设置图表ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置卡片标题。
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 取得或设置卡片图表数据信息列表。
    /// </summary>
    public List<CardChartInfo> Charts { get; set; } = [];
}

/// <summary>
/// 卡片图表数据信息类。
/// </summary>
public class CardChartInfo
{
    /// <summary>
    /// 取得或设置图表数据名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置图表数据类型。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置图表数据标题。
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 取得或设置图表数据信息列表。
    /// </summary>
    public ChartDataInfo[] Datas { get; set; }
}