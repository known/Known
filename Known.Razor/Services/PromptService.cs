namespace Known.Razor;

partial class UIService
{
    public void Prompt(string title, Size size, Action<RenderTreeBuilder> content, Action<dynamic> action)
    {
        Show<PromptForm>(title, size, action: attr =>
        {
            attr.Set(c => c.Content, content)
                .Set(c => c.Action, action);
        });
    }
}

class PromptForm : Form
{
    [Parameter] public Action<RenderTreeBuilder> Content { get; set; }
    [Parameter] public Action<dynamic> Action { get; set; }

    protected override void BuildFields(RenderTreeBuilder builder) => Content?.Invoke(builder);

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button(FormButton.OK, Callback(OnAction));
        builder.Button(FormButton.Cancel, Callback(OnCancel));
    }

    private void OnAction() => Submit(Action);
}