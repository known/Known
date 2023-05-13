namespace Known.Razor;

public class FormComponent : PageComponent
{
    protected Form form;

    [Parameter] public bool IsDialog { get; set; }
    [Parameter] public object Model { get; set; }
    [Parameter] public Action<Result> OnSuccess { get; set; }

    protected virtual bool IsTable => !Context.IsMobile;
    protected string FormStyle { get; set; } = "inline";
    protected string ButtonStyle { get; set; } = "inline";
    protected string CheckFields { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            UI.BindEnter();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        if (Model == null)
            builder.Div("", "Loading...");
        else
            BuildPage(builder, Model);
    }

    protected void BuildPage(RenderTreeBuilder builder, object model)
    {
        builder.ComponentRef<Form>(attr =>
        {
            attr.Add(nameof(Form.IsTable), IsTable)
                .Add(nameof(Form.ReadOnly), ReadOnly)
                .Add(nameof(Form.Style), FormStyle)
                .Add(nameof(Form.Model), model)
                .Add(nameof(Form.CheckFields), CheckFields)
                .Add(nameof(Form.ChildContent), BuildTree(BuildFields));
            builder.Reference<Form>(value => form = value);
        });
        builder.Div($"form-button {ButtonStyle}", attr => BuildButtons(builder));
    }

    protected virtual void BuildFields(RenderTreeBuilder builder) { }
    protected virtual void BuildButtons(RenderTreeBuilder builder) => builder.Button(FormButton.Close, Callback(OnCancel));

    protected void Submit(Func<dynamic, Result> action, Action<Result> onSuccess = null)
    {
        form.Submit(action, OnSubmitted(onSuccess));
    }

    protected void SubmitAsync(Func<dynamic, Task<Result>> action, Action<Result> onSuccess = null)
    {
        form.SubmitAsync(action, OnSubmitted(onSuccess));
    }

    protected void SubmitFilesAsync(Func<MultipartFormDataContent, Task<Result>> action, Action<Result> onSuccess = null)
    {
        form.SubmitFilesAsync(action, OnSubmitted(onSuccess));
    }

    protected void SubmitFilesAsync(Func<UploadFormInfo, Task<Result>> action, Action<Result> onSuccess = null)
    {
        form.SubmitFilesAsync(action, OnSubmitted(onSuccess));
    }

    protected virtual void OnOK()
    {
        form.Submit(data =>
        {
            var result = Result.Success("", data);
            OnSuccess?.Invoke(result);
            UI.CloseDialog();
        });
    }

    protected virtual void OnCancel() => UI.CloseDialog();
    protected void OnSuccessed() => OnSuccess?.Invoke(Result.Success(""));

    private Action<Result> OnSubmitted(Action<Result> onSuccess)
    {
        return result =>
        {
            onSuccess?.Invoke(result);
            OnSuccess?.Invoke(result);
        };
    }
}

public class BaseForm<T> : FormComponent
{
    protected T TModel => (T)Model;

    protected override void BuildFields(RenderTreeBuilder builder) => BuildFields(new FieldBuilder<T>(builder));
    protected virtual void BuildFields(FieldBuilder<T> builder) { }

    protected void BuildFlowLog(RenderTreeBuilder builder, string bizId, int colSpan)
    {
        builder.FormList<FlowLogGrid>("流程记录", colSpan, null, attr =>
        {
            attr.Set(c => c.BizId, bizId);
        });
    }
}