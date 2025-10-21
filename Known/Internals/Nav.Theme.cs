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
        builder.Component<Switch>()
               .Set(c => c.CheckedChildren, "🌜")
               .Set(c => c.UnCheckedChildren, "🌞")
               .Set(c => c.Value, Context.Local?.Theme == "dark")
               .Set(c => c.OnChange, this.Callback<bool>(ThemeChangedAsync))
               .Build();
    }

    private async Task ThemeChangedAsync(bool isDark)
    {
        Context.Local.Theme = isDark ? "dark" : "default";
        if (CurrentUser != null)
        {
            Context.UserSetting.Theme = Context.Local.Theme;
            await Admin.SaveUserSettingAsync(Context.UserSetting);
        }
        await JS.SetLocalInfoAsync(Context.Local);
    }
}