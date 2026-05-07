namespace Known.Internals;

/// <summary>
/// 自定义Ant主题组件类。
/// </summary>
[NavPlugin(Language.NavTheme, "retweet", Category = Language.Component, Sort = 6)]
public class NavTheme : BaseNav
{
    /// <summary>
    /// 取得主题组件标题。
    /// </summary>
    protected override string Title => Language.NavTheme;

    /// <summary>
    /// 呈现主题组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var isDark = Context.Local?.Theme == "dark";
        builder.Component<KIcon>()
               .Set(c => c.Title, Title)
               .Set(c => c.Icon, isDark ? "fa fa-moon" : "fa fa-sun")
               .Set(c => c.OnClick, this.Callback<MouseEventArgs>(async _ => await ChangeThemeAsync(!isDark)))
               .Build();
    }
}
