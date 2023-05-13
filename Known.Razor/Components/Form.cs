namespace Known.Razor.Components;

public class Form : BaseComponent
{
    public Form()
    {
        FormContext = new FormContext();
    }

    [Parameter] public bool IsTable { get; set; }
    [Parameter] public string Style { get; set; }
    [Parameter] public object Model { get; set; }
    [Parameter] public string CheckFields { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    internal FormContext FormContext { get; }
    public dynamic Data => FormContext.Data;
    public Dictionary<string, Field> Fields => FormContext.Fields;
    public T FieldAs<T>(string id) where T : Field => FormContext.FieldAs<T>(id);

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        FormContext.IsTableForm = IsTable;
        FormContext.ReadOnly = ReadOnly;
        FormContext.Model = Model;
        FormContext.CheckFields = CheckFields;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div($"form {Style}", attr =>
        {
            builder.Component<CascadingValue<FormContext>>(attr =>
            {
                attr.Set(c => c.IsFixed, false)
                    .Set(c => c.Value, FormContext)
                    .Set(c => c.ChildContent, ChildContent);
            });
        });
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

    internal void Submit(Func<dynamic, Result> action, Action<Result> onSuccess)
    {
        if (!Validate())
            return;

        Result result = action.Invoke(Data);
        UI.Result(result, () => onSuccess?.Invoke(result));
    }

    internal async void SubmitAsync(Func<dynamic, Task<Result>> action, Action<Result> onSuccess = null)
    {
        if (!Validate())
            return;

        Result result = await action.Invoke(Data);
        UI.Result(result, () => onSuccess?.Invoke(result));
    }

    internal async void SubmitFilesAsync(Func<MultipartFormDataContent, Task<Result>> action, Action<Result> onSuccess = null)
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
        UI.Result(result, () => onSuccess?.Invoke(result));
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
        }, onSuccess);
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