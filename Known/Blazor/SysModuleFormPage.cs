using Known.Designers;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysModuleFormPage : BaseComponent
{
    [CascadingParameter] private SysModuleForm Form { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<UIDesigner>()
               .Set(c => c.Type, "Page")
               .Set(c => c.EntityType, Form.EntityType)
               .Set(c => c.Columns, Form.Columns)
               .Build();
    }
}