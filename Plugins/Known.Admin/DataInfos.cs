namespace Known;

/// <summary>
/// 设置信息类。
/// </summary>
public class SettingInfo
{
    /// <summary>
    /// 构造函数，创建一个设置信息类的实例。
    /// </summary>
    public SettingInfo()
    {
        Id = Utils.GetGuid();
    }

    /// <summary>
    /// 取得或设置ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置创建人。
    /// </summary>
    public string CreateBy { get; set; }

    /// <summary>
    /// 取得或设置业务类型。
    /// </summary>
    public string BizType { get; set; }

    /// <summary>
    /// 取得或设置业务名称。
    /// </summary>
    public string BizName { get; set; }

    /// <summary>
    /// 取得或设置业务数据。
    /// </summary>
    public string BizData { get; set; }

    /// <summary>
    /// 将业务数据JSON转换成泛型对象。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>泛型对象。</returns>
    public T DataAs<T>() => Utils.FromJson<T>(BizData);
}

/// <summary>
/// 数据数量统计信息类。
/// </summary>
public class CountInfo
{
    /// <summary>
    /// 取得或设置统计字段1。
    /// </summary>
    public string Field1 { get; set; }

    /// <summary>
    /// 取得或设置统计字段2。
    /// </summary>
    public string Field2 { get; set; }

    /// <summary>
    /// 取得或设置统计字段3。
    /// </summary>
    public string Field3 { get; set; }

    /// <summary>
    /// 取得或设置统计数量。
    /// </summary>
    public int TotalCount { get; set; }
}

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