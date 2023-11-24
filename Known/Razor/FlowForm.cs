using Known.Extensions;
using Known.WorkFlows;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Razor;

public class FlowForm<TItem> : BaseComponent where TItem : FlowEntity, new()
{
    [Parameter] public FormModel<TItem> Model { get; set; }
    [Parameter] public Action<RenderTreeBuilder> Content { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("form-content", () => Content?.Invoke(builder));
        builder.Div("form-action", () =>
        {
            if (Model.Data.CanSubmit)
                UI.BuildButton(builder, new ButtonOption { Type = "primary", Text = "提交审核" });
            if (!Model.IsView)
                UI.BuildButton(builder, new ButtonOption { Type = "primary", Text = "确定", OnClick = Callback<MouseEventArgs>(OnSave) });
            UI.BuildButton(builder, new ButtonOption { Text = "取消", OnClick = Callback<MouseEventArgs>(OnClose) });
        });
    }

    private async void OnSave(MouseEventArgs args) => await Model.SaveAsync();
    private async void OnClose(MouseEventArgs args) => await Model.CloseAsync();
}