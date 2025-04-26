using AntDesign;

namespace Known.Internals;

/// <summary>
/// 页面主内容组件类。
/// </summary>
public partial class MainBody
{
    private ReloadContainer reload;
    private string TabsClass => CssBuilder.Default("kui-nav-tabs").AddClass("is-top", Context.UserSetting.IsTopTab).BuildClass();
    [Inject] private ReuseTabsService Service { get; set; }

    /// <summary>
    /// 取得或设置子组件内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// 重新加载页面。
    /// </summary>
    public void ReloadPage()
    {
        if (Context.UserSetting.MultiTab)
            Service.ReloadPage();
        else
            reload?.Reload();
    }
}