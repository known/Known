using Microsoft.AspNetCore.Components;

namespace Known.Server;

public partial class Routes
{
    [Parameter] public UIContext Context { get; set; }
}