using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Blazor;

public class BaseForm : BaseComponent
{
    protected override async Task OnInitializedAsync()
    {
        try
        {
            await base.OnInitializedAsync();
            await OnInitFormAsync();
        }
        catch (Exception ex)
        {
            await Error?.HandleAsync(ex);
        }
    }

    protected virtual Task OnInitFormAsync() => Task.CompletedTask;

    protected override async void BuildRenderTree(RenderTreeBuilder builder)
    {
        try
        {
            BuildForm(builder);
        }
        catch (Exception ex)
        {
            await Error?.HandleAsync(ex);
        }
    }

    protected virtual void BuildForm(RenderTreeBuilder builder) { }
}

public class BaseForm<TItem> : BaseForm where TItem : class, new()
{
    [Parameter] public bool ShowAction { get; set; }
    [Parameter] public FormModel<TItem> Model { get; set; }

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        UI.BuildForm(builder, Model);
        if (ShowAction && !Model.IsView)
        {
            builder.FormAction(() =>
            {
                UI.Button(builder, new ActionInfo(Context, "OK", ""), this.Callback<MouseEventArgs>(OnSaveAsync));
                UI.Button(builder, new ActionInfo(Context, "Cancel", ""), this.Callback<MouseEventArgs>(OnCloseAsync));
            });
        }
    }

    private async void OnSaveAsync(MouseEventArgs args) => await Model.SaveAsync();
    private async void OnCloseAsync(MouseEventArgs args) => await Model.CloseAsync();
}