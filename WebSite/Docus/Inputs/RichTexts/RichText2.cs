namespace WebSite.Docus.Inputs.RichTexts;

class RichText2 : BaseComponent
{
    private string? message;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<RichText>("内容：", "RichText2").ValueChanged(OnValueChanged).Build();
        builder.Div("tips", message);
    }

    private void OnValueChanged(string value)
    {
        message = value;
        StateChanged();
    }
}