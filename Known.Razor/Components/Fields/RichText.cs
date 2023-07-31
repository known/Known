namespace Known.Razor.Components.Fields;

public class RichText : Field
{
    private IJSObjectReference editor;

    [Parameter] public object Option { get; set; }

    public override void SetValue(object value)
    {
        editor?.InvokeVoidAsync("txt.html", value?.ToString());
        SetFieldValue(value);
        StateChanged();
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
        if (firstRender)
            editor = await UI.InitEditor(Id, Option);

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override ValueTask DisposeAsync(bool disposing)
    {
        CallbackHelper.Dispose(Id);
        return base.DisposeAsync(disposing);
    }

    protected override void BuildText(RenderTreeBuilder builder)
    {
        builder.Markup(Value);
    }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        builder.Div("editor", attr => attr.Id(Id));
    }
}