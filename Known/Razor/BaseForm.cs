using Microsoft.AspNetCore.Components;

namespace Known.Razor;

public class BaseForm : BaseComponent
{
    protected override async Task OnInitializedAsync()
    {
        await OnInitFormAsync();
        await base.OnInitializedAsync();
    }

    protected virtual Task OnInitFormAsync() => Task.CompletedTask;
}

public class BaseForm<TItem> : BaseForm where TItem : class, new()
{
    [Parameter] public FormModel<TItem> Model { get; set; }
}