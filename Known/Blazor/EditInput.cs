using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class EditInput : BaseComponent
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
                builder.Link(Language.OK, this.Callback(OnSaveClick));
                builder.Link(Language.Cancel, this.Callback(() => isEdit = false));
            }
            else
            {
                builder.Span(Value);
                builder.Link(Language.Edit, this.Callback(() => isEdit = true));
            }
        });
    }

    private void OnSaveClick()
    {
        OnSave?.Invoke(Value);
        isEdit = false;
    }
}