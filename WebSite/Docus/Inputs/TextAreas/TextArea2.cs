namespace WebSite.Docus.Inputs.TextAreas;

class TextArea2 : BaseComponent
{
    private string? message;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //ValueChanged事件
        builder.Field<KTextArea>("示例：", "TextArea").ValueChanged(OnValueChanged).Build();
        builder.Div("tips", message);
    }

    private void OnValueChanged(string value)
    {
        message = value;
        StateChanged();
    }
}