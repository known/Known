using AntDesign;

namespace Known.Internals;

/// <summary>
/// 自定义Ant主题组件类。
/// </summary>
[NavPlugin("主题", "retweet", Category = "组件", Sort = 5)]
public class NavTheme : BaseNav
{
    /// <summary>
    /// 呈现主题组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Component<Switch>()
               .Set(c => c.CheckedChildren, "🌜")
               .Set(c => c.UnCheckedChildren, "🌞")
               .Set(c => c.Value, Context.Theme == "dark")
               .Set(c => c.OnChange, this.Callback<bool>(ThemeChangedAsync))
               .Build();
    }

    /// <summary>
    /// 组件呈现后异步操作。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            Context.Theme = await JS.SetThemeAsync(null);
            await StateChangedAsync();
        }
    }

    private async Task ThemeChangedAsync(bool isDark)
    {
        Context.Theme = isDark ? "dark" : "default";
        await JS.SetThemeAsync(Context.Theme);
    }
}