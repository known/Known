using AntDesign;

namespace Known.Pages;

/// <summary>
/// 用户个人中心页面组件类。
/// </summary>
[StreamRendering]
[Route("/profile")]
public class SysUserProfile : BasePage<UserInfo>, IReuseTabsPage
{
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
    }

    /// <summary>
    /// 获取标签页标题模板。
    /// </summary>
    /// <returns>标签页标题模板。</returns>
    public RenderFragment GetPageTitle()
    {
        return this.BuildTree(b =>
        {
            b.Icon("user");
            b.Span(Language["Nav.Profile"]);
        });
    }

    private void BuildUserInfo(RenderTreeBuilder builder) => builder.Component<SysUserProfileInfo>().Build();
    private void BuildUserTabs(RenderTreeBuilder builder) => builder.Component<SysUserProfileTabs>().Build();
}