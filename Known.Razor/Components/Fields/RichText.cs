namespace Known.Razor.Components.Fields;

public class RichText : Field
{
    private bool isInit = false;
    private IJSObjectReference editor;

    [Parameter] public object Option { get; set; }

    internal override void SetFieldVisible(bool visible)
    {
        isInit = visible;
        base.SetFieldVisible(visible);
    }

    internal override void SetFieldReadOnly(bool readOnly)
    {
        isInit = !readOnly;
        base.SetFieldReadOnly(readOnly);
    }

    internal override void SetFieldEnabled(bool enabled)
    {
        if (enabled)
            editor?.InvokeVoidAsync("enable");
        else
            editor?.InvokeVoidAsync("disable");
        base.SetFieldEnabled(enabled);
    }

    internal override void SetFieldValue(object value)
    {
        SetHtml(value?.ToString());
        base.SetFieldValue(value);
    }

    protected override void OnInitialized()
    {
        CallbackHelper.Register(Id, "rich.onchange", new Func<Dictionary<string, object>, Task>(ChangeValue));
        base.OnInitialized();
    }

    private Task ChangeValue(Dictionary<string, object> param)
    {
        Value = param["html"].ToString();
        OnValueChange();
        return Task.CompletedTask;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender || isInit)
        {
            if (isInit)
                Destroy();
            editor = await UI.InitEditor(Id, Option);
            if (!string.IsNullOrWhiteSpace(Value))
                SetHtml(Value);
            isInit = false;
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override ValueTask DisposeAsync(bool disposing)
    {
        Destroy();
        CallbackHelper.Dispose(Id);
        return base.DisposeAsync(disposing);
    }

    protected override void BuildText(RenderTreeBuilder builder) => builder.Markup(Value);
    protected override void BuildInput(RenderTreeBuilder builder) => builder.Div("editor", attr => attr.Id(Id));
    private void SetHtml(string html) => editor?.InvokeVoidAsync("txt.html", html);
    private void Destroy() => editor?.InvokeVoidAsync("destroy");
}