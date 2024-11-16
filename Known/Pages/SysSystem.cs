namespace Known.Pages;

/// <summary>
/// 关于系统页面组件类。
/// </summary>
[StreamRendering]
[Route("/sys/info")]
public class SysSystem : BaseTabPage
{
    internal SystemDataInfo Data { get; private set; }

    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Data = await System.GetSystemDataAsync();

        Tab.AddTab("SystemInfo", b => b.Component<SysSystemInfo>().Build());
        Tab.AddTab("SystemSetting", b => b.Component<SysSystemSetting>().Build());
        foreach (var item in UIConfig.SystemTabs)
        {
            Tab.AddTab(item.Key, item.Value);
        }
    }

    /// <summary>
    /// 构建页面内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildPage(RenderTreeBuilder builder) => builder.Cascading(this, base.BuildPage);

    internal async Task<Result> SaveSystemAsync(SystemInfo info)
    {
        var result = await System.SaveSystemAsync(info);
        if (result.IsValid)
            Context.System = info;
        return result;
    }

    internal Task<Result> SaveKeyAsync(SystemInfo info) => System.SaveKeyAsync(info);
}