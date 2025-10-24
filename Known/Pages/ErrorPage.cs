namespace Known.Pages;

/// <summary>
/// 错误页面组件类。
/// </summary>
[Anonymous]
[Route("/error/{code}")]
public class ErrorPage : BasePage
{
    /// <summary>
    /// 取得或设置错误页面代码。
    /// </summary>
    [Parameter] public string Code { get; set; }

    /// <inheritdoc />
    public override RenderFragment GetPageTitle()
    {
        return GetPageTitle("close-circle", Language.Error);
    }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        UIConfig.Errors.TryGetValue(Code, out ErrorConfigInfo error);
        builder.Div("kui-card kui-error", () =>
        {
            builder.Result(Code, Code, error?.Description);
            if (error != null && error.Template != null)
                error.Template?.Invoke(builder);
            else
                builder.Button(Language.BackHome, this.Callback<MouseEventArgs>(e => Context.GoHomePage()));
        });
    }
}