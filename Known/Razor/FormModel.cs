using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

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
    public Dictionary<string, List<IBrowserFile>> Files { get; } = [];
    public Type Type { get; internal set; }
    public Func<bool> OnValidate { get; set; }
    public Func<Task> OnClose { get; set; }
    internal Func<TItem, Task<Result>> OnSave { get; set; }
    internal Func<UploadInfo<TItem>, Task<Result>> OnSaveFile { get; set; }

    public bool Validate()
    {
        if (OnValidate == null)
            return true;

        return OnValidate.Invoke();
    }

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
                await OnClose?.Invoke();
            await Page.RefreshAsync();
        });
    }
}

public class TabFormModel<TItem> : FormModel<TItem> where TItem : class, new()
{
    internal TabFormModel(FormOption option, PageModel<TItem> page) : base(option, page)
    {
        Tabs = [];
    }

    public List<ItemModel> Tabs { get; }
}

public class ItemModel
{
    public string Title { get; set; }
    public RenderFragment Content { get; set; }
}

public class FormOption
{
    public double? Width { get; set; }
    public bool NoFooter { get; set; }
}