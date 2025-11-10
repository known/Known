namespace Known.Components;

/// <summary>
/// 图表组件类，配置参考echarts.js。
/// </summary>
public class KChart : BaseComponent
{
    private string ClassName => CssBuilder.Default("kui-chart").AddClass(Class).BuildClass();

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
    /// 取得或设置图表标题配置对象。
    /// </summary>
    [Parameter] public object Title { get; set; }

    /// <summary>
    /// 取得或设置图表数据提示配置对象。
    /// </summary>
    [Parameter] public object Tooltip { get; set; }

    /// <summary>
    /// 取得或设置图表的图例配置对象。
    /// </summary>
    [Parameter] public object Legend { get; set; }

    /// <summary>
    /// 取得或设置图表网格配置对象。
    /// </summary>
    [Parameter] public object Grid { get; set; }

    /// <summary>
    /// 取得或设置图表X轴对象。
    /// </summary>
    [Parameter] public object XAxis { get; set; }

    /// <summary>
    /// 取得或设置图表Y轴对象。
    /// </summary>
    [Parameter] public object YAxis { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (!Visible)
            return;

        builder.Div().Id(Id).Class(ClassName).Style(Style).Close();
    }

    /// <summary>
    /// 异步显示图表。
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    public ValueTask ShowAsync(object option)
    {
        return JSRuntime.InvokeVoidAsync("KUtils.showECharts", Id, option);
    }

    /// <summary>
    /// 异步显示折线图。
    /// </summary>
    /// <param name="title">折线图标题。</param>
    /// <param name="datas">折线图数据集合。</param>
    /// <returns></returns>
    public ValueTask ShowLineAsync(string title, ChartDataInfo[] datas)
    {
        if (!Visible)
            return ValueTask.CompletedTask;

        object series = null;
        string[] categories = [];
        if (datas != null && datas.Length > 0)
        {
            categories = datas[0].Series.Keys.ToArray();
            series = datas.Select(d => new
            {
                type = "line",
                name = d.Name,
                data = d.Series.Values.ToArray(),
                smooth = true
            }).ToArray();
        }

        var option = new
        {
            title = Title ?? new { text = title, left = "center" },
            tooltip = Tooltip ?? new { },
            legend = Legend ?? new { y = "bottom" },
            grid = Grid ?? new { top = "13%", left = "8%", right = "3%", bottom = "13%" },
            xAxis = XAxis ?? new { type = "category", categories },
            yAxis = YAxis ?? new { },
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
    public ValueTask ShowBarAsync(string title, ChartDataInfo[] datas)
    {
        if (!Visible)
            return ValueTask.CompletedTask;

        object series = null;
        string[] categories = [];
        if (datas != null && datas.Length > 0)
        {
            categories = datas[0].Series.Keys.ToArray();
            series = datas.Select(d => new
            {
                type = "bar",
                name = d.Name,
                data = d.Series.Values.ToArray(),
                label = new { show = true, position = "top" }
            }).ToArray();
        }

        var option = new
        {
            title = Title ?? new { text = title, left = "center" },
            tooltip = Tooltip ?? new { },
            legend = Legend ?? new { y = "bottom" },
            grid = Grid ?? new { top ="13%", left ="8%", right = "3%", bottom = "13%" },
            xAxis = XAxis ?? new { type = "category", data = categories },
            yAxis = YAxis ?? new { },
            series
        };

        return ShowAsync(option);
    }
}