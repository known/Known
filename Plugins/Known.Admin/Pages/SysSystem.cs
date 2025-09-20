namespace Known.Pages;

[Route("/sys/info")]
[Menu(Constants.System, "关于系统", "info-circle", 1)]
//[PagePlugin("关于系统", "info-circle", PagePluginType.Module, AdminLanguage.SystemManage, Sort = 4)]
public class SysSystem : BaseTabPage
{
    private ISystemService Service;

    internal SystemDataInfo Model { get; set; } = new SystemDataInfo();

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<ISystemService>();

        foreach (var item in UIConfig.SystemTabs.OrderBy(t => t.Value.Id))
        {
            Tab.AddTab(item.Key, b => b.DynamicComponent(item.Value));
        }
    }

    protected override void BuildPage(RenderTreeBuilder builder) => builder.Cascading(this, base.BuildPage);

    internal Task<SystemDataInfo> GetSystemDataAsync() => Service.GetSystemDataAsync();

    internal Task<Result> SaveSystemAsync(SystemInfo info)
    {
        Config.System = info;
        return Service.SaveSystemAsync(info);
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