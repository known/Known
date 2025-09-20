namespace Known.Pages;

[Route("/sys/info")]
[Menu(Constants.System, "关于系统", "info-circle", 1)]
//[PagePlugin("关于系统", "info-circle", PagePluginType.Module, AdminLanguage.SystemManage, Sort = 4)]
public class SysSystem : BaseTabPage
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        foreach (var item in UIConfig.SystemTabs.OrderBy(t => t.Value.Id))
        {
            Tab.AddTab(item.Key, b => b.DynamicComponent(item.Value));
        }
    }
}