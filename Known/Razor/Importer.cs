using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

class Importer : BaseComponent
{
    [Parameter] public ImportFormInfo Model { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("danger", "提示: 请上传单个txt或Excel格式附件！");
    }
}