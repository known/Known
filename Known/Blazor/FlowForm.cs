using Known.Extensions;
using Known.WorkFlows;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Blazor;

public class FlowForm<TItem> : BaseComponent where TItem : FlowEntity, new()
{
    [Parameter] public FormModel<TItem> Model { get; set; }
    [Parameter] public Action<RenderTreeBuilder> Content { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("form-content", () => Content?.Invoke(builder));
        
        if (Model.IsView)
            return;

        builder.Div("form-action", () =>
        {
            UI.BuildButton(builder, new ActionInfo("OK", "") { OnClick = Callback<MouseEventArgs>(OnSave) });
            UI.BuildButton(builder, new ActionInfo("Cancel", "") { OnClick = Callback<MouseEventArgs>(OnClose) });
        });
    }

    private async void OnSave(MouseEventArgs args) => await Model.SaveAsync();
    private async void OnClose(MouseEventArgs args) => await Model.CloseAsync();
}