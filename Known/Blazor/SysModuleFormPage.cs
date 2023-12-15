using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysModuleFormPage : BaseComponent
{
    [CascadingParameter] private SysModuleForm Form { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //TODO：模块页面配置组件开发
        builder.Div("kui-module-page", () =>
        {
            builder.Div("left", () =>
            {
                builder.Div("", () =>
                {
                });
            });
            builder.Div("right", () =>
            {

            });
        });
    }

    private void OnModelChanged(string obj)
    {
    }
}