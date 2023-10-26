namespace WebSite.Docus.Inputs.RichTexts;

class RichText2 : BaseComponent
{
    private string? message;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<KRichText>("内容：", "RichText2")
               .ValueChanged(OnValueChanged)
               .Set(f => f.Option, new
               {
                   Placeholder = "请输入通知内容",
                   UploadImgShowBase64 = true //支持贴图
               }).Build();
        builder.Div("tips", message);
    }

    private void OnValueChanged(string value)
    {
        message = value;
        StateChanged();
    }
}