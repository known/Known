namespace Known.Pages;

/// <summary>
/// 关于系统页面组件类。
/// </summary>
[StreamRendering]
[Route("/sys/info")]
public class SysSystem : BaseTabPage
{
    internal SystemDataInfo Model { get; private set; }

    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Model = await Admin.GetSystemDataAsync();

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

    internal Task<Result> SaveSystemAsync(SystemInfo info)
    {
        Config.System = info;
        return Admin.SaveSystemAsync(info);
    }

    internal Task<Result> SaveKeyAsync(SystemInfo info) => Admin.SaveProductKeyAsync(info);
}