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
        Logger.Exception(LogTarget.FrontEnd, Value?.CurrentUser, ex);
        builder.Div("kui-wrapper", () =>
        {
            builder.Component<AntDesign.Result>()
                   .Set(c => c.Status, ResultStatus.Http500)
                   .Set(c => c.Title, "500")
                   .Set(c => c.SubTitle, ex.Message)
                   .Set(c => c.Extra, b => BuildExtra(b, ex))
                   .Build();
        });
    };

    private void BuildExtra(RenderTreeBuilder builder, Exception ex)
    {
        if (Config.IsDebug)
            builder.Pre().Child(ex.ToString());

        builder.Div().Child(() =>
        {
            builder.Button("重新加载", this.Callback<MouseEventArgs>(e => Value?.Refresh()));
            builder.Button("返回首页", this.Callback<MouseEventArgs>(e => Value?.GoHomePage(null, true)));
        });
    }
}