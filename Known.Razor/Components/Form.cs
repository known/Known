namespace Known.Razor.Components;

public class Form : BaseComponent
{
    private bool isInitialized;

    public Form()
    {
        isInitialized = false;
        FormContext = new FormContext();
    }

    [Parameter] public string Style { get; set; }
    [Parameter] public object Model { get; set; }
    [Parameter] public Action<Result> OnSuccess { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    internal FormContext FormContext { get; }
    protected bool IsTable { get; set; }
    protected string CheckFields { get; set; }
    public dynamic Data => FormContext.Data;
    public Dictionary<string, Field> Fields => FormContext.Fields;
    public T FieldAs<T>(string id) where T : Field => FormContext.FieldAs<T>(id);

    protected virtual Task InitFormAsync() => Task.CompletedTask;

    protected override async Task OnInitializedAsync()
    {
        await AddVisitLogAsync();
        await InitFormAsync();
        isInitialized = true;
    }
    
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        FormContext.IsTableForm = IsTable;
        FormContext.ReadOnly = ReadOnly;
        FormContext.Model = Model;
        FormContext.CheckFields = CheckFields;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            UI.BindEnter();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!isInitialized)
            return;

        if (!Context.Check.IsCheckKey)
        {
            BuildAuthorize(builder);
            return;
        }

        var css = CssBuilder.Default("form").AddClass(Style).Build();
        builder.Div(css, attr =>
        {
            if (ChildContent == null)
            {
                builder.Div("form-body", attr =>
                {
                    builder.Component<CascadingValue<FormContext>>(attr =>
                    {
                        attr.Set(c => c.IsFixed, false)
                            .Set(c => c.Value, FormContext)
                            .Set(c => c.ChildContent, BuildTree(BuildFields));
                    });
                });
                builder.Div("form-button", attr => BuildButtons(builder));
            }
            else
            {
                builder.Component<CascadingValue<FormContext>>(attr =>
                {
                    attr.Set(c => c.IsFixed, false)
                        .Set(c => c.Value, FormContext)
                        .Set(c => c.ChildContent, ChildContent);
                });
            }
        });
    }

    protected virtual void BuildFields(RenderTreeBuilder builder) { }
    protected virtual void BuildButtons(RenderTreeBuilder builder) => builder.Button(FormButton.Close, Callback(OnCancel));

    protected virtual void OnOK()
    {
        Submit(data =>
        {
            var result = Result.Success("", data);
            OnSuccess?.Invoke(result);
            UI.CloseDialog();
        });
    }

    protected virtual void OnCancel() => UI.CloseDialog();

    protected bool HasButton(ButtonInfo button)
    {
        var user = CurrentUser;
        if (user == null)
            return false;

        return button.IsInMenu(Id);
    }

    public bool Validate() => FormContext.Validate();
    public bool ValidateCheck(bool isPass) => FormContext.ValidateCheck(isPass);
    public void Clear() => FormContext.Clear();

    public void SetData(object data)
    {
        Model = data;
        FormContext.SetData(data);
        StateChanged();
    }

    public void Submit(Action<dynamic> action)
    {
        if (!Validate())
            return;

        action.Invoke(Data);
    }

    public async void SubmitAsync(Func<dynamic, Task<Result>> action, Action<Result> onSuccess = null)
    {
        if (!Validate())
            return;

        Result result = await action.Invoke(Data);
        UI.Result(result, OnSubmitted(result, onSuccess));
    }

    public async void SubmitFilesAsync(Func<MultipartFormDataContent, Task<Result>> action, Action<Result> onSuccess = null)
    {
        if (!Validate())
            return;

        using var content = new MultipartFormDataContent();
        var json = Utils.ToJson(Data);
        var modelContent = new StringContent(json);
        content.Add(modelContent, "\"model\"");
        AddFiles(content);
        AddMultiFiles(content);
        Result result = await action.Invoke(content);
        UI.Result(result, OnSubmitted(result, onSuccess));
    }

    internal void SubmitFilesAsync(Func<UploadFormInfo, Task<Result>> action, Action<Result> onSuccess = null)
    {
        SubmitAsync(data =>
        {
            var info = new UploadFormInfo
            {
                Model = data,
                Files = GetFiles(),
                MultiFiles = GetMultiFiles()
            };
            return action.Invoke(info);
        }, OnSubmitted(onSuccess));
    }

    private Action OnSubmitted(Result result, Action<Result> onSuccess)
    {
        return () =>
        {
            onSuccess?.Invoke(result);
            OnSuccess?.Invoke(result);
        };
    }

    private Action<Result> OnSubmitted(Action<Result> onSuccess)
    {
        return result =>
        {
            onSuccess?.Invoke(result);
            OnSuccess?.Invoke(result);
        };
    }

    private void AddFiles(MultipartFormDataContent content)
    {
        var files = FormContext.Files;
        foreach (var item in files)
        {
            AddFileContent(content, item.Key, item.Value);
        }
    }

    private void AddMultiFiles(MultipartFormDataContent content)
    {
        var files = FormContext.MultiFiles;
        foreach (var item in files)
        {
            foreach (var file in item.Value)
            {
                AddFileContent(content, item.Key, file);
            }
        }
    }

    private static void AddFileContent(MultipartFormDataContent content, string key, IBrowserFile file)
    {
        var fileContent = new StreamContent(file.OpenReadStream(Upload.MaxLength));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, $"\"file{key}\"", file.Name);
    }

    private Dictionary<string, IAttachFile> GetFiles()
    {
        var dicFiles = FormContext.Files;
        var files = new Dictionary<string, IAttachFile>();
        foreach (var item in dicFiles)
        {
            files.Add(item.Key, new BlazorAttachFile(item.Value));
        }
        return files;
    }

    private Dictionary<string, List<IAttachFile>> GetMultiFiles()
    {
        var dicFiles = FormContext.MultiFiles;
        var files = new Dictionary<string, List<IAttachFile>>();
        foreach (var item in dicFiles)
        {
            var values = GetAttachFiles(item.Value);
            files.Add(item.Key, values);
        }
        return files;
    }

    private static List<IAttachFile> GetAttachFiles(List<IBrowserFile> bFiles)
    {
        var files = new List<IAttachFile>();
        foreach (var item in bFiles)
        {
            files.Add(new BlazorAttachFile(item));
        }
        return files;
    }
}

public class BaseForm<T> : Form
{
    public BaseForm()
    {
        IsTable = true;
        Style = "inline";
    }

    protected T TModel => (T)Model;

    protected Field Field(Expression<Func<T, object>> selector)
    {
        var property = TypeHelper.Property(selector);
        return Fields[property.Name];
    }

    protected TField Field<TField>(Expression<Func<T, object>> selector) where TField : Field
    {
        var property = TypeHelper.Property(selector);
        return FieldAs<TField>(property.Name);
    }

    protected override void BuildFields(RenderTreeBuilder builder) => BuildFields(new FieldBuilder<T>(builder));
    protected virtual void BuildFields(FieldBuilder<T> builder) { }
}