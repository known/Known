namespace Known.Razor.Components.Fields;

public class RichText : Field
{
    private bool isInit = false;
    private IJSObjectReference editor;
    private object option;

    public RichText()
    {
        Style = "editor";
    }

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
        base.SetFieldValue(value);
        SetHtml(Value);
    }

    internal override void ClearFieldValue()
    {
        //editor?.InvokeVoidAsync("txt.clear");
        base.ClearFieldValue();
        SetHtml(Value);
    }

    protected override void OnInitialized()
    {
        option = new
        {
            Focus = false,
            UploadImgServer = $"{Context.Http?.BaseAddress}File/UploadEditorImage"
        };
        CallbackHelper.Register(Id, "rich.onchange", new Func<Dictionary<string, object>, Task>(ChangeValue));
        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        Height = null;
        if (IsReadOnly)
        {
            var option = Utils.MapTo<Dictionary<string, object>>(Option);
            if (option != null)
            {
                if (option.ContainsKey("Height"))
                    Height = option.GetValue<int>("Height");
                else if (option.ContainsKey("height"))
                    Height = option.GetValue<int>("height");
            }
        }
        base.OnParametersSet();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender || isInit)
        {
            if (isInit)
                Destroy();

            var option1 = option.Merge(Option);
            editor = await UI.InitEditor(Id, option1);
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
    protected override void BuildInput(RenderTreeBuilder builder) => builder.Div(attr => attr.Id(Id));

    private Task ChangeValue(Dictionary<string, object> param)
    {
        Value = param["html"].ToString();
        OnValueChange();
        return Task.CompletedTask;
    }

    private void SetHtml(string html) => editor?.InvokeVoidAsync("txt.html", html ?? "");
    private void Destroy() => editor?.InvokeVoidAsync("destroy");
}