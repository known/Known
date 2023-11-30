using System.ComponentModel;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Forms;

namespace Known.Blazor;

public class FormModel<TItem> where TItem : class, new()
{
    private List<FormRow<TItem>> rows;

    internal FormModel(PageModel<TItem> page)
    {
        Page = page;
    }

    internal FormModel(PageModel<TItem> page, FormOption option)
    {
        Page = page;
        Option = option;
        Type = Config.FormTypes.GetValueOrDefault($"{typeof(TItem).Name}Form");
    }

    public List<FormRow<TItem>> Rows
    {
        get
        {
            if (rows == null)
            {
                rows = [];
                var columns = Page.Table.AllColumns.Where(c => c.IsForm).ToList();
                var rowNos = columns.Select(c => c.Row).Distinct().ToList();
                if (rowNos.Count == 1)
                {
                    foreach (var item in columns)
                    {
                        var row = new FormRow<TItem>();
                        row.Fields.Add(new FieldModel<TItem>(this, item));
                        rows.Add(row);
                    }
                }
                else
                {
                    foreach (var rowNo in rowNos)
                    {
                        var row = new FormRow<TItem>();
                        var fields = columns.Where(c => c.Row == rowNo).OrderBy(c => c.Column).ToList();
                        foreach (var item in fields)
                        {
                            row.Fields.Add(new FieldModel<TItem>(this, item));
                        }
                        rows.Add(row);
                    }
                }
            }
            return rows;
        }
    }

    public PageModel<TItem> Page { get; }
    public FormOption Option { get; }
    public bool IsView { get; internal set; }
    public string Title { get; internal set; }
    public TItem Data { get; set; }
    public Dictionary<string, List<CodeInfo>> Codes { get; } = [];
    public Dictionary<string, List<IBrowserFile>> Files { get; } = [];
    public Type Type { get; internal set; }
    public Func<bool> OnValidate { get; set; }
    public Func<Task> OnClose { get; set; }
    internal FormType FormType { get; set; }
    internal Func<TItem, Task<Result>> OnSave { get; set; }
    internal Func<UploadInfo<TItem>, Task<Result>> OnSaveFile { get; set; }

    internal List<CodeInfo> GetCodes(ColumnInfo column)
    {
        if (Codes.TryGetValue(column.Category, out List<CodeInfo> value))
            return value;

        return null;
    }

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

public class FormRow<TItem> where TItem : class, new()
{
    public List<FieldModel<TItem>> Fields { get; } = [];
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