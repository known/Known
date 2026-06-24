namespace Known.Components;

/// <summary>
/// 卡片页面组件类。
/// </summary>
public partial class CardPage
{
    private string ClassName => CssBuilder.Default("kui-card kui-card-page").AddClass(Class).BuildClass();

    /// <summary>
    /// 取得或设置卡片工具条。
    /// </summary>
    [Parameter] public RenderFragment Toolbar { get; set; }

    /// <summary>
    /// 取得或设置卡片内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
}