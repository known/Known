using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class PageFooter : ComponentBase
{
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