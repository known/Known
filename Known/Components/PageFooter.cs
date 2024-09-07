namespace Known.Components;

/// <summary>
/// 框架默认页面页脚组件类。
/// </summary>
public class PageFooter : ComponentBase
{
    /// <summary>
    /// 呈现页面页面组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Markup($@"<div>
            <span>&copy;2020-{DateTime.Now:yyyy}</span>
            <span>Powered By</span>
            <a href=""{Config.SiteUrl}"" target=""_blank"">Known</a>
        </div>
        <div style=""margin-top:10px;"">
            <a href=""{Config.GiteeUrl}"" target=""_blank"">GITEE</a>
            <span>&sdot;</span>
            <a href=""{Config.GithubUrl}"" target=""_blank"">GITHUB</a>
        </div>");
    }
}