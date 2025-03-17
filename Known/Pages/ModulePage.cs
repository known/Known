namespace Known.Pages;

/// <summary>
/// 系统模块管理页面组件类。
/// </summary>
[Route("/dev/modules")]
[DevPlugin("模块管理", "appstore-add", Sort = 1)]
public class ModulePage : BaseTabPage
{
    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        if (!CurrentUser.IsSystemAdmin())
        {
            Navigation.GoErrorPage("403");
            return;
        }

        await base.OnInitPageAsync();
        foreach (var item in UIConfig.ModuleTabs.OrderBy(t => t.Value.Id))
        {
            Tab.AddTab(item.Key, b => b.DynamicComponent(item.Value));
        }
    }
}

class SysModuleList : BasePage
{
    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        if (UIConfig.ModulePageType != null)
            builder.Component(UIConfig.ModulePageType);
        else
            builder.Component<ModuleList>().Build();
    }
}