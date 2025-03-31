namespace Known.Components;

/// <summary>
/// 图标卡片组件类。
/// </summary>
public partial class ChartCard
{
    private KChart chart;
    private bool isLoad;

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
        isLoad = true;
        await StateChangedAsync();
        var info = option?.Charts?.FirstOrDefault();
        if (info?.Type == "Line")
            await chart?.ShowLineAsync(info?.Title, info?.Datas);
        else if (info?.Type == "Bar")
            await chart?.ShowBarAsync(info?.Title, info?.Datas);
        else if (info?.Type == "Pie")
            await chart?.ShowPieAsync(info?.Title, info?.Datas);
    }
}