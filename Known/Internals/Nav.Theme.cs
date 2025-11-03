using AntDesign;

namespace Known.Internals;

/// <summary>
/// 自定义Ant主题组件类。
/// </summary>
[NavPlugin(Language.NavTheme, "retweet", Category = Language.Component, Sort = 5)]
public class NavTheme : BaseNav
{
    /// <summary>
    /// 呈现主题组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var theme = Context.Local?.Theme;
        builder.Component<Switch>()
               .Set(c => c.CheckedChildren, "🌜")
               .Set(c => c.UnCheckedChildren, "🌞")
               .Set(c => c.Value, theme == "dark")
               .Set(c => c.OnChange, this.Callback<bool>(ChangeThemeAsync))
               .Build();
    }
}