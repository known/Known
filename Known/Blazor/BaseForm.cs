using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class BaseForm : BaseComponent
{
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await OnInitFormAsync();
    }

    protected virtual Task OnInitFormAsync() => Task.CompletedTask;
}

public class BaseForm<TItem> : BaseForm where TItem : class, new()
{
    [Parameter] public FormModel<TItem> Model { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder) => UI.BuildForm(builder, Model);
}