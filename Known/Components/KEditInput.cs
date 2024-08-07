﻿namespace Known.Components;

public class KEditInput : BaseComponent
{
    private bool isEdit;

    [Parameter] public string Value { get; set; }
    [Parameter] public Action<string> OnSave { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
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