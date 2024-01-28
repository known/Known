using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class AdvancedSearch : BaseComponent
{
    public List<QueryInfo> Query { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Text(Context.Module.Name);
    }
}