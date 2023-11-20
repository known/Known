using AntDesign;
using Known.Extensions;
using Known.Razor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace KnownAntDesign.Components;

public class EditInput : BaseComponent
{
    private bool isEdit;

    [Parameter] public string Value { get; set; }
    [Parameter] public Action<string> OnSave { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("edit-input", () =>
        {
            if (isEdit)
            {
                builder.Component<Input<string>>()
                       .Set(c => c.Value, Value)
                       .Set(c => c.ValueChanged, Callback<string>(value => Value = value))
                       .Build();
                builder.Link("确定", Callback(OnSaveClick));
                builder.Link("取消", Callback(() => isEdit = false));
            }
            else
            {
                builder.Span(Value);
                builder.Link("编辑", Callback(() => isEdit = true));
            }
        });
    }

    private void OnSaveClick()
    {
        OnSave?.Invoke(Value);
        isEdit = false;
    }
}