namespace Known.Pages;

/// <summary>
/// 错误页面组件类。
/// </summary>
[StreamRendering]
[AllowAnonymous]
[Route("/error/{code}")]
public class ErrorPage : BasePage
{
    /// <summary>
    /// 取得或设置错误页面代码。
    /// </summary>
    [Parameter] public string Code { get; set; }

    /// <summary>
    /// 呈现错误页面内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        UIConfig.Errors.TryGetValue(Code, out ErrorConfigInfo error);
        builder.Div("kui-card kui-error", () =>
        {
            builder.Result(Code, error?.Description);
            if (error != null && error.Template != null)
                error.Template?.Invoke(builder);
            else
                builder.Button(Language["Button.BackHome"], this.Callback<MouseEventArgs>(e => Navigation.NavigateTo("/")));
        });
    }
}