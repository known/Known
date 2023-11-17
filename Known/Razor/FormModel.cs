using Known.Extensions;

namespace Known.Razor;

public class FormModel<TItem> where TItem : class, new()
{
    internal FormModel(FormOption option, PageModel<TItem> page)
    {
        Option = option;
        Page = page;
        Fields = page.Table.AllColumns.Where(c => c.IsForm).Select(c => new FieldModel<TItem>(this, c));
        Type = Config.FormTypes.GetValueOrDefault($"{typeof(TItem).Name}Form");
    }

    public FormOption Option { get; }
    public PageModel<TItem> Page { get; }
    public IEnumerable<FieldModel<TItem>> Fields { get; }
    public bool IsView { get; internal set; }
    public string Title { get; internal set; }
    public TItem Data { get; set; }
    public Type Type { get; internal set; }
    public Func<bool> OnValidate { get; set; }
    public Func<Task> OnClose { get; set; }
    public Func<TItem, Task<Result>> OnSave { get; internal set; }
    public Func<UploadInfo<TItem>, Task<Result>> OnSaveFile { get; internal set; }

    public async Task SaveAsync()
    {
        if (OnValidate != null)
        {
            if (!OnValidate.Invoke())
                return;
        }

        //TODO：保存附件表单
        Result result;
        if (OnSaveFile != null)
        {
            var info = new UploadInfo<TItem>(Data);
            result = await OnSaveFile?.Invoke(info);
        }
        else
        {
            result = await OnSave?.Invoke(Data);
        }
        Page.UI.Result(result, async () =>
        {
            if (result.IsClose)
                await OnClose?.Invoke();
            await Page.RefreshAsync();
        });
    }
}

public class FormOption
{
    public double? Width { get; set; }
    public bool NoFooter { get; set; }
}