using BootstrapBlazor.Components;
using Known.Blazor;
using Microsoft.AspNetCore.Components;

namespace Known.BootBlazor.Components;

class BootTree : Tree
{
    [Parameter] public TreeModel Model { get; set; }
}