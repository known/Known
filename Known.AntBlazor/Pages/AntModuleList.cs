namespace Known.AntBlazor.Pages;

/// <summary>
/// 系统模块管理页面组件类。
/// </summary>
[StreamRendering]
[Route("/sys/modules")]
public class AntModuleList : SysModuleList, IReuseTabsPage
{
    /// <summary>
    /// 获取标签页标题模板。
    /// </summary>
    /// <returns>标签页标题模板。</returns>
    public RenderFragment GetPageTitle()
    {
        return this.BuildTree(b =>
        {
            b.Icon("appstore-add");
            b.Span(Language.SysModule);
        });
    }
}