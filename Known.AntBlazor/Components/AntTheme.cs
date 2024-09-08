namespace Known.AntBlazor.Components;

/// <summary>
/// 自定义Ant主题组件类。
/// </summary>
public class AntTheme : BaseComponent
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
               .Set(c => c.OnChange, this.Callback<bool>(ThemeChanged))
               .Build();
    }

    private async void ThemeChanged(bool isDark)
    {
        Context.Theme = isDark ? "dark" : "light";
        await JS.SetCurrentThemeAsync(Context.Theme);
    }
}