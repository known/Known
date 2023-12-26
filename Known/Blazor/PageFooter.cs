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
            <a href=""http://known.pumantech.com"" target=""_blank"">Known</a>
        </div>
        <div style=""margin-top:10px;"">
            <a href=""https://gitee.com/known/Known"" target=""_blank"">GITEE</a>
            <span>&sdot;</span>
            <a href=""https://github.com/known/Known"" target=""_blank"">GITHUB</a>
        </div>");
    }
}