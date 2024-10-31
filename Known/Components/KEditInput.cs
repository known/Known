namespace Known.Components;

/// <summary>
/// 文本编辑框组件类。
/// </summary>
public class KEditInput : BaseComponent
{
    private bool isEdit;

    /// <summary>
    /// 取得或设置文本值。
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// 取得或设置保存文本委托方法。
    /// </summary>
    [Parameter] public Func<string, Task> OnSave { get; set; }

    /// <summary>
    /// 呈现文本编辑框组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
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