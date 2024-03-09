using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class Error : BaseComponent
{
    [Parameter] public RenderFragment ChildContent { get; set; }
    [Parameter] public Func<Exception, Task> OnError { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Cascading<Error>(this, ChildContent);
    }

    public Task HandleAsync(Exception exception)
    {
        return OnError?.Invoke(exception);
    }
}