namespace WebSite.Docus.Inputs.Texts;

class Text2 : BaseComponent
{
    private string? message;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //ValueChanged事件
        builder.Field<KText>("示例：", "Text").ValueChanged(OnValueChanged).Build();
        builder.Div("tips", message);
    }

    private void OnValueChanged(string value)
    {
        message = value;
        StateChanged();
    }
}