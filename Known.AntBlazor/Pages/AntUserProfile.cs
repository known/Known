namespace Known.AntBlazor.Pages;

/// <summary>
/// 用户个人中心页面组件类。
/// </summary>
[StreamRendering]
[Route("/profile")]
public class AntUserProfile : SysUserProfile, IReuseTabsPage
{
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
}