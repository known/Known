namespace Known.Blazor;

public class KError : BaseComponent
{
    [Parameter] public RenderFragment ChildContent { get; set; }
    [Parameter] public Func<Exception, Task> OnError { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Cascading(this, ChildContent);
    }

    public Task HandleAsync(Exception exception)
    {
        return OnError?.Invoke(exception);
    }
}