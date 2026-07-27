namespace Known.Pages;

/// <summary>
/// 错误页面组件类。
/// </summary>
[Anonymous]
[Route("/dberror")]
[DisplayName(Language.Error)]
[Layout(typeof(EmptyLayout))]
public class DbErrorPage : BaseComponent
{
    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-card kui-dberror", () =>
        {
            builder.Result("500", "500", Language["数据库连接不可用，请联系管理员！"]);
        });
    }
}

/// <summary>
/// 错误页面组件类。
/// </summary>
[Anonymous]
[Route("/error/{code}")]
[DisplayName(Language.Error)]
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
            builder.Result(Code, Code, Language[error?.Description]);
            if (error != null)
            {
                if (error.Template != null)
                    error.Template?.Invoke(builder);
                else if (error.IsBackHome)
                    builder.Button(Language.BackHome, this.Callback<MouseEventArgs>(e => Context.GoHomePage()));
            }
            else
            {
                builder.Button(Language.BackHome, this.Callback<MouseEventArgs>(e => Context.GoHomePage()));
            }
        });
    }
}