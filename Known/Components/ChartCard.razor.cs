namespace Known.Components;

/// <summary>
/// 图标卡片组件类。
/// </summary>
public partial class ChartCard
{
    private KChart chart;
    private bool isLoad;
    private string current;
    private string tabs;
    private List<CardChartInfo> charts = [];

    /// <summary>
    /// 取得或设置卡片标题。
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// 异步设置图标选项数据。
    /// </summary>
    /// <param name="option">图标选项数据。</param>
    /// <returns></returns>
    public async Task SetOptionAsync(ChartCardOption option)
    {
        if (!Visible)
            return;

        charts = option?.Charts ?? [];
        tabs = string.Empty;
        current = string.Empty;
        if (charts.Count > 1)
        {
            tabs = string.Join(",", charts.Select(c => c.Name));
            current = charts[0].Name;
        }
        isLoad = true;
        await StateChangedAsync();
        await OnChanged(current);
    }

    private async Task OnChanged(string value)
    {
        var info = charts.FirstOrDefault(c => c.Name == value);
        if (info?.Type == "Line")
            await chart.ShowLineAsync(info?.Title, info?.Datas);
        else if (info?.Type == "Bar")
            await chart.ShowBarAsync(info?.Title, info?.Datas);
    }
}