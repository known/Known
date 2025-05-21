namespace Known.Components;

/// <summary>
/// 图表组件类，配置参考highcharts.js。
/// </summary>
public partial class KChart
{
    private string ClassName => CssBuilder.Default("chart").AddClass(Class).BuildClass();

    /// <summary>
    /// 取得或设置图表类型。
    /// </summary>
    [Parameter] public string Type { get; set; }

    /// <summary>
    /// 取得或设置图表宽度。
    /// </summary>
    [Parameter] public int? Width { get; set; }

    /// <summary>
    /// 取得或设置图表高度。
    /// </summary>
    [Parameter] public int? Height { get; set; }

    /// <summary>
    /// 取得或设置图表X轴对象。
    /// </summary>
    [Parameter] public object XAxis { get; set; }

    /// <summary>
    /// 取得或设置图表Y轴对象。
    /// </summary>
    [Parameter] public object YAxis { get; set; }

    /// <summary>
    /// 取得或设置图表的图例配置对象。
    /// </summary>
    [Parameter] public object Legend { get; set; }

    /// <summary>
    /// 取得或设置图表数据提示配置对象。
    /// </summary>
    [Parameter] public object Tooltip { get; set; }

    /// <summary>
    /// 取得或设置图表绘制配置对象。
    /// </summary>
    [Parameter] public object PlotOptions { get; set; }

    /// <summary>
    /// 异步显示图表。
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    public Task ShowAsync(object option)
    {
        return JS.ShowChartAsync(Id, option);
    }

    /// <summary>
    /// 异步显示折线图。
    /// </summary>
    /// <param name="title">折线图标题。</param>
    /// <param name="datas">折线图数据集合。</param>
    /// <returns></returns>
    public Task ShowLineAsync(string title, ChartDataInfo[] datas)
    {
        if (!Visible)
            return Task.CompletedTask;

        object series = null;
        if (datas != null && datas.Length > 0)
        {
            series = datas.Select(d => new
            {
                name = d.Name,
                data = d.Series.Values.ToArray(),
                showInLegend = datas.Length > 1
            }).ToArray();
        }

        var option = new
        {
            credits = new { enabled = false },
            chart = new { width = Width, height = Height },
            title = new { text = title },
            xAxis = XAxis ?? new { categories = datas[0].Series.Keys.ToArray() },
            yAxis = YAxis ?? new { },
            legend = Legend ?? new { },
            tooltip = Tooltip ?? new { },
            plotOptions = PlotOptions ?? new { },
            series
        };

        return ShowAsync(option);
    }

    /// <summary>
    /// 异步显示柱状图。
    /// </summary>
    /// <param name="title">柱状图标题。</param>
    /// <param name="datas">柱状图数据集合。</param>
    /// <returns></returns>
    public Task ShowBarAsync(string title, ChartDataInfo[] datas)
    {
        if (!Visible)
            return Task.CompletedTask;

        object series = null;
        if (datas != null && datas.Length > 0)
        {
            series = datas.Select(d => new
            {
                name = d.Name,
                data = d.Series.Values.ToArray()
            }).ToArray();
        }

        var option = new
        {
            credits = new { enabled = false },
            chart = new { type = Type ?? "column", backgroundColor = "rgba(0,0,0,0)", width = Width, height = Height },
            title = new { text = title },
            xAxis = XAxis ?? new { categories = datas[0].Series.Keys.ToArray() },
            yAxis = YAxis ?? new { },
            legend = Legend ?? new { },
            tooltip = Tooltip ?? new { },
            plotOptions = PlotOptions ?? new { },
            series
        };

        return ShowAsync(option);
    }

    /// <summary>
    /// 异步显示饼图。
    /// </summary>
    /// <param name="title">饼图标题。</param>
    /// <param name="datas">饼图数据集合。</param>
    /// <returns></returns>
    public Task ShowPieAsync(string title, ChartDataInfo[] datas)
    {
        if (!Visible)
            return Task.CompletedTask;

        object series = null;
        if (datas != null && datas.Length > 0)
        {
            series = datas.Select(d => new
            {
                name = d.Name,
                data = d.Series.Select(s => new { name = s.Key, y = s.Value }).ToArray()
            }).ToArray();
        }

        var option = new
        {
            credits = new { enabled = false },
            chart = new { type = "pie", backgroundColor = "rgba(0,0,0,0)", width = Width, height = Height },
            title = new { text = title },
            legend = Legend ?? new { },
            tooltip = Tooltip ?? new { pointFormat = "{series.name}: <b>{point.percentage:.1f}%</b>" },
            plotOptions = PlotOptions ?? new { },
            series
        };

        return ShowAsync(option);
    }
}