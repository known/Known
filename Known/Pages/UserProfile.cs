using AntDesign;

namespace Known.Pages;

/// <summary>
/// 用户个人中心页面组件类。
/// </summary>
[Route("/profile")]
[ReuseTabsPage(Title = "个人中心")]
public class UserProfile : BasePage<UserInfo>
{
    private TabModel Tab;

    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Page.Type = PageType.Column;
        Page.Spans = "28";
        Page.AddItem("kui-card kui-p10", BuildUserInfo);
        Page.AddItem("kui-card", BuildUserTabs);

        Tab = new TabModel(this);
        foreach (var item in UIConfig.UserTabs.OrderBy(t => t.Value.Id))
        {
            Tab.AddTab(item.Key, b => b.DynamicComponent(item.Value));
        }
    }

    private void BuildUserInfo(RenderTreeBuilder builder) => builder.DynamicComponent(UIConfig.UserProfileType);
    private void BuildUserTabs(RenderTreeBuilder builder) => builder.Tabs(Tab);
}