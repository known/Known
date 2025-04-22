namespace Known;

/// <summary>
/// 自动页面数据交互信息类。
/// </summary>
/// <typeparam name="TData">交互数据泛型类型。</typeparam>
public class AutoInfo<TData>
{
    /// <summary>
    /// 取得或设置页面模块ID/建表实体名/生成代码路径。
    /// </summary>
    public string PageId { get; set; }

    /// <summary>
    /// 取得或设置页面插件ID/生成代码类型。
    /// </summary>
    public string PluginId { get; set; }

    /// <summary>
    /// 取得或设置交互数据泛型对象/建表脚本/生成代码。
    /// </summary>
    public TData Data { get; set; }
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

/// <summary>
/// 图表数据信息类。
/// </summary>
public class ChartDataInfo
{
    /// <summary>
    /// 取得或设置图表名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置图表数据字典。
    /// </summary>
    public Dictionary<string, object> Series { get; set; }
}