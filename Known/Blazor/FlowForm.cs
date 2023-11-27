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

        if (Model.IsView && Model.FlowAction == FlowAction.None)
            return;

        if (Model.FlowAction != FlowAction.None)
            BuildFlowAction(builder);

        builder.Div("form-action", () =>
        {
            UI.BuildButton(builder, new ActionInfo("OK", "") { OnClick = Callback<MouseEventArgs>(OnSave) });
            UI.BuildButton(builder, new ActionInfo("Cancel", "") { OnClick = Callback<MouseEventArgs>(OnClose) });
        });
    }

    private void BuildFlowAction(RenderTreeBuilder builder)
    {
        builder.Div("form-flow", () =>
        {
            var action = Model.FlowAction.GetDescription();
            builder.Span("title", $"{action}流程");
            switch (Model.FlowAction)
            {
                case FlowAction.Submit:

                    break;
                case FlowAction.Revoke:
                    break;
                case FlowAction.Verify:
                    break;
                case FlowAction.Repeat:
                    break;
            }
        });
    }

    private async void OnSave(MouseEventArgs args)
    {
        var info = new FlowFormInfo();
        switch (Model.FlowAction)
        {
            case FlowAction.Submit:
                await Platform.Flow.SubmitFlowAsync(info);
                break;
            case FlowAction.Revoke:
                await Platform.Flow.RevokeFlowAsync(info);
                break;
            case FlowAction.Verify:
                break;
            case FlowAction.Repeat:
                break;
            default:
                await Model.SaveAsync();
                break;
        }
    }

    private async void OnClose(MouseEventArgs args) => await Model.CloseAsync();
}