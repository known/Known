using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class KAuthPanel : BaseComponent
{
    [Parameter] public RenderFragment ChildContent { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (!Config.IsAuth)
            BuildAuthorize(builder);
        else
            ChildContent(builder);
    }

    private void BuildAuthorize(RenderTreeBuilder builder)
    {
        builder.Component<SysActive>()
               .Set(c => c.OnCheck, isCheck =>
               {
                   Config.IsAuth = isCheck;
                   StateChanged();
               })
               .Build();
    }
}