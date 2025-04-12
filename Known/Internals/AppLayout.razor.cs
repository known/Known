
namespace Known.Internals;

/// <summary>
/// 移动端模板组件类。
/// </summary>
public partial class AppLayout
{
    private List<MenuInfo> TabItems = [];
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

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        TabItems = Config.AppMenus?.Where(m => m.Target == "Tab").OrderBy(m => m.Sort).ToList();
    }
}