namespace Known.Pages;

/// <summary>
/// 数据字典表单组件类。
/// </summary>
public partial class DictionaryForm
{
    /// <inheritdoc />
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        var items = Utils.FromJson<List<CodeName>>(Model.Data.Child);
        if (items != null && items.Count > 0)
            ListItems.AddRange(items);
        Model.OnSaving = data =>
        {
            data.Child = Utils.ToJson(ListItems);
            return Task.FromResult(true);
        };
    }

    private Task OnFilesChanged(List<FileDataInfo> files)
    {
        Model.Files[nameof(SysDictionary.Extension)] = files;
        return Task.CompletedTask;
    }

    private Task OnPasteAsync()
    {
        return JSRuntime.PasteTextAsync(text =>
        {
            var lines = text.Split(Environment.NewLine.ToCharArray())
                            .Where(l => !string.IsNullOrWhiteSpace(l))
                            .ToList();
            Model.Fields.Clear();
            foreach (var line in lines)
            {
                var items = line.Contains('\t') ? line.Split('\t') : line.Split('|');
                var info = new CodeName();
                if (items.Length > 0) info.Code = items[0].Trim();
                if (items.Length > 1) info.Name = items[1].Trim();
                ListItems.Add(info);
            }
        });
    }
}

/// <summary>
/// 代码名称信息类。
/// </summary>
public class CodeName
{
    /// <summary>
    /// 取得或设置代码。
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    public string Name { get; set; }
}

/// <summary>
/// 数据字典类型表单组件类。
/// </summary>
public class DictionaryTypeForm : AntForm<SysDictionary> { }

/// <summary>
/// 代码名称表格组件类。
/// </summary>
public class CodeNameTable : AntTable<CodeName> { }