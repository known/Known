using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class KEditInput : BaseComponent
{
    private bool isEdit;

    [Parameter] public string Value { get; set; }
    [Parameter] public Action<string> OnSave { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-edit-input", () =>
        {
            if (isEdit)
            {
                UI.BuildText(builder, new InputModel<string>
                {
                    Value = Value,
                    ValueChanged = this.Callback<string>(value => Value = value)
                });
                builder.Link("确定", this.Callback(OnSaveClick));
                builder.Link("取消", this.Callback(() => isEdit = false));
            }
            else
            {
                builder.Span(Value);
                builder.Link("编辑", this.Callback(() => isEdit = true));
            }
        });
    }

    private void OnSaveClick()
    {
        OnSave?.Invoke(Value);
        isEdit = false;
    }
}