using System.ComponentModel;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Forms;

namespace Known.Blazor;

public class FormModel<TItem> where TItem : class, new()
{
    internal FormModel(PageModel<TItem> page)
    {
        Page = page;
    }

    internal FormModel(PageModel<TItem> page, FormOption option)
    {
        Page = page;
        Option = option;
        Fields = page.Table.AllColumns.Where(c => c.IsForm).Select(c => new FieldModel<TItem>(this, c));
        Type = Config.FormTypes.GetValueOrDefault($"{typeof(TItem).Name}Form");
    }

    public PageModel<TItem> Page { get; }
    public FormOption Option { get; }
    public IEnumerable<FieldModel<TItem>> Fields { get; }
    public bool IsView { get; internal set; }
    public string Title { get; internal set; }
    public TItem Data { get; set; }
    public Dictionary<string, List<IBrowserFile>> Files { get; } = [];
    public Type Type { get; internal set; }
    public Func<bool> OnValidate { get; set; }
    public Func<Task> OnClose { get; set; }
    internal FormType FormType { get; set; }
    internal Func<TItem, Task<Result>> OnSave { get; set; }
    internal Func<UploadInfo<TItem>, Task<Result>> OnSaveFile { get; set; }

    public bool Validate()
    {
        if (OnValidate == null)
            return true;

        return OnValidate.Invoke();
    }

    public Task CloseAsync() => OnClose?.Invoke();

    public async Task SaveAsync(bool isClose = false)
    {
        if (!Validate())
            return;

        Result result;
        if (OnSaveFile != null)
        {
            var info = new UploadInfo<TItem>(Data);
            foreach (var file in Files)
            {
                info.Files[file.Key] = [];
                foreach (var item in file.Value)
                {
                    info.Files[file.Key].Add(new BlazorAttachFile(item));
                }
            }
            result = await OnSaveFile?.Invoke(info);
        }
        else
        {
            result = await OnSave?.Invoke(Data);
        }
        Page.UI.Result(result, async () =>
        {
            if (result.IsClose || isClose)
                await CloseAsync();
            await Page.RefreshAsync();
        });
    }
}

public class FormOption
{
    public double? Width { get; set; }
    public bool NoFooter { get; set; }
}

enum FormType
{
    [Description("查看")] View,
    [Description("提交")] Submit,
    [Description("审核")] Verify
}