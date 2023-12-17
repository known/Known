using Known.Designers;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysModuleFormModel : BaseComponent
{
    [CascadingParameter] private SysModuleForm Form { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<UIDesigner>()
               .Set(c => c.Type, "Page")
               .Build();
    }
}