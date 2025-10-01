namespace Known.Pages;

public partial class DictionaryForm
{
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

public class CodeName
{
    public string Code { get; set; }
    public string Name { get; set; }
}

public class DictionaryTypeForm : AntForm<SysDictionary> { }
public class CodeNameTable : AntTable<CodeName> { }