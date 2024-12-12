using AntDesign;

namespace Known.Pages;

/// <summary>
/// 系统开发中心页面组件类。
/// </summary>
[StreamRendering]
[Route("/development")]
public class Development : BaseTabPage, IReuseTabsPage
{
    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitPageAsync()
    {
        if (!CurrentUser.IsSystemAdmin())
        {
            Navigation.GoErrorPage("403");
            return;
        }

        await base.OnInitPageAsync();
        Tab.Class = "kui-development";
        foreach (var item in UIConfig.DevelopTabs)
        {
            Tab.AddTab(item.Key, b => b.DynamicComponent(item.Value));
        }
    }

    /// <summary>
    /// 获取标签页标题模板。
    /// </summary>
    /// <returns>标签页标题模板。</returns>
    public RenderFragment GetPageTitle()
    {
        return this.BuildTree(b =>
        {
            b.Icon("appstore-add");
            b.Span(Language["Nav.Development"]);
        });
    }
}