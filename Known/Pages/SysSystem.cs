namespace Known.Pages;

/// <summary>
/// 关于系统页面组件类。
/// </summary>
[Route("/sys/info")]
[Menu(Constants.System, "关于系统", "info-circle", 1)]
public class SysSystem : BaseTabPage
{
    internal SystemDataInfo Model { get; set; } = new SystemDataInfo();

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        foreach (var item in UIConfig.SystemTabs.OrderBy(t => t.Value.Id))
        {
            Tab.AddTab(item.Key, b => b.DynamicComponent(item.Value));
        }
    }

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder) => builder.Cascading(this, base.BuildPage);

    internal Task<Result> SaveSystemAsync(SystemInfo info)
    {
        Config.System = info;
        return Admin.SaveSystemAsync(info);
    }

    internal Task<Result> SaveKeyAsync(SystemInfo info)
    {
        return Admin.SaveProductKeyAsync(new ActiveInfo
        {
            ProductId = info.ProductId,
            ProductKey = info.ProductKey
        });
    }
}