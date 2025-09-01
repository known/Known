using AntDesign;

namespace Known.Components;

/// <summary>
/// 上下文组件类。
/// </summary>
public partial class KContext
{
    /// <summary>
    /// 取得或设置上下文值。
    /// </summary>
    [Parameter] public UIContext Value { get; set; }

    /// <summary>
    /// 取得或设置内容模板。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    private RenderFragment<Exception> ErrorContent => ex => builder =>
    {
        var className = CssBuilder.Default("kui-wrapper").AddClass("kui-app", Value?.IsMobileApp == true).BuildClass();
        var status = ex.IsNotAuthorized() ? ResultStatus.Http403 : ResultStatus.Http500;
        var title = status == ResultStatus.Http403 ? "403" : "500";
        Logger.Exception(LogTarget.FrontEnd, Value?.CurrentUser, ex);
        builder.Div(className, () =>
        {
            builder.Component<AntDesign.Result>()
                   .Set(c => c.Status, status)
                   .Set(c => c.Title, title)
                   .Set(c => c.SubTitle, ex.Message)
                   .Set(c => c.Extra, b => BuildExtra(b, ex))
                   .Build();
        });
    };

    private void BuildExtra(RenderTreeBuilder builder, Exception ex)
    {
        if (Config.IsDebug && !ex.IsNotAuthorized())
            builder.Pre().Child(ex.ToString());

        builder.Div().Child(() =>
        {
            builder.Button(Language.ReLoad, this.Callback<MouseEventArgs>(e => Value?.Refresh()));
            builder.Button(Language.BackHome, this.Callback<MouseEventArgs>(e => Value?.GoHomePage(null, true)));
        });
    }
}