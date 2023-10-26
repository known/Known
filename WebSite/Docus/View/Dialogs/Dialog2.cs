namespace WebSite.Docus.View.Dialogs;

class Dialog2 : BaseComponent
{
    private string? formData;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Button("处理", Callback(OnPrompt), StyleType.Primary);
        builder.Span("tips", formData);
    }

    private void OnPrompt()
    {
        UI.Prompt("异常处理", new(300, 200), builder =>
        {
            builder.Field<KTextArea>("原因", "Reason", true).Build();
        }, 
        data =>
        {
            formData = Utils.ToJson(data);
            StateChanged();
            UI.CloseDialog();
        });
    }
}