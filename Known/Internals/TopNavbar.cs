namespace Known.Internals;

/// <summary>
/// 全局顶部导航工具条组件类。
/// </summary>
public class TopNavbar : BaseComponent
{
    /// <summary>
    /// 取得或设置按钮点击事件委托。
    /// </summary>
    [Parameter] public Action<string> OnMenuClick { get; set; }

    /// <summary>
    /// 呈现组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Ul("kui-nav", () =>
        {
            builder.Cascading(this, b =>
            {
                if (UIConfig.IsEditMode && UIConfig.EditNavType != null)
                    b.Li().Child(() => b.DynamicComponent(UIConfig.EditNavType));

                if (UIConfig.TopNavType != null)
                    b.DynamicComponent(UIConfig.TopNavType);
                else
                    BuildTopNavbar(b);

                if (UIConfig.EnableEdit && CurrentUser?.IsSystemAdmin() == true)
                {
                    var className = UIConfig.IsEditMode ? "edit" : "";
                    b.Li().Class(className).Child(() => b.Component<NavEditMode>().Build());
                }

                b.Li(() => b.Component<NavSetting>().Build());
            });
        });
    }

    private static void BuildTopNavbar(RenderTreeBuilder builder)
    {
        builder.Li(() => builder.Component<NavHome>().Build());
        builder.Li(() => builder.Component<NavFontSize>().Build());
        builder.Li(() => builder.Component<NavFullScreen>().Build());
        builder.Li(() => builder.Component<NavLanguage>().Build());
        builder.Li(() => builder.Component<NavUser>().Build());
        builder.Li(() => builder.Component<NavTheme>().Build());
    }
}