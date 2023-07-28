namespace WebSite.Docus.Inputs.TextAreas;

class TextArea3 : BaseComponent
{
    private TextArea? textArea;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<FieldControl>()
               .Set(c => c.OnVisibleChanged, OnVisibleChanged)
               .Set(c => c.OnEnabledChanged, OnEnabledChanged)
               .Set(c => c.OnReadOnlyChanged, OnReadOnlyChanged)
               .Set(c => c.SetValue, SetValue)
               .Set(c => c.GetValue, GetValue)
               .Build();

        builder.Field<TextArea>("示例：", "TextArea").Build(value => textArea = value);
    }

    private void OnVisibleChanged(bool value) => textArea?.SetVisible(value);
    private void OnEnabledChanged(bool value) => textArea?.SetEnabled(value);
    private void OnReadOnlyChanged(bool value) => textArea?.SetReadOnly(value);
    private void SetValue() => textArea?.SetValue("孙膑");
    private string? GetValue() => textArea?.Value;
}