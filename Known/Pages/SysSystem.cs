namespace Known.Pages;

/// <summary>
/// 关于系统页面组件类。
/// </summary>
[StreamRendering]
[Route("/sys/info")]
[Menu(Constants.System, "关于系统", "info-circle", 1)]
public class SysSystem : BaseTabPage
{
    internal SystemDataInfo Model { get; set; } = new SystemDataInfo();

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Tab.AddTab("SystemInfo", b => b.Component<SysSystemInfo>().Build());
        Tab.AddTab("SystemSetting", b => b.Component<SysSystemSetting>().Build());
        foreach (var item in UIConfig.SystemTabs)
        {
            Tab.AddTab(item.Key, item.Value);
        }
    }

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder) => builder.Cascading(this, base.BuildPage);

    internal Task<Result> SaveSystemAsync(SystemInfo info)
    {
        Config.System = info;
        return Admin.SaveSystemAsync(info);
    }

    internal Task<Result> SaveKeyAsync(SystemInfo info) => Admin.SaveProductKeyAsync(info);
}