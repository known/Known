namespace Known.Components;

/// <summary>
/// 移动端模板组件类。
/// </summary>
public partial class AppLayout
{
    private bool IsHome => Context.Url == "/app";
    private bool IsTab => Context.Current?.Target == "Tab";
    private string PageClass => CssBuilder.Default("kui-app-page")
                                          .AddClass("nav", !IsHome)
                                          .AddClass("tab", IsTab)
                                          .BuildClass();

    /// <summary>
    /// 取得或设置组件子内容模板。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
}