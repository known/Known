using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

class SysModuleFormForm : BaseComponent
{
    [CascadingParameter] private SysModuleForm Form { get; set; }

    //TODO：模块表单配置组件开发
}