using Known.Extensions;

namespace Known.Blazor;

public class PageModel<TItem> where TItem : class, new()
{
    internal PageModel(BasePage<TItem> page)
    {
        UI = page.UI;
        Page = page;
        Name = page.Name;
        Tools = page.Tools;
        Table = new TableModel<TItem>(this);
        Form = new FormOption();
    }

    internal IUIService UI { get; }
    internal BasePage<TItem> Page { get; }

    public string Name { get; }
    public FormOption Form { get; }
    public TableModel<TItem> Table { get; }
    public List<ActionInfo> Tools { get; }
    public Action<ActionInfo> OnToolClick { get; internal set; }
    public Action StateChanged { get; set; }
    public TreeModel Tree { get; set; }

    public Func<TItem, string> FormTitle { get; set; }

    public async Task RefreshAsync()
    {
        if (Tree != null)
        {
            await Tree.RefreshAsync();
            StateChanged?.Invoke();
        }

        await Table?.RefreshAsync();
    }

    public void ViewForm(TItem row)
    {
        var title = GetFormTitle(row);
        UI.ShowForm(new FormModel<TItem>(Form, this)
        {
            IsView = true,
            Title = $"查看{title}",
            Data = row
        });
    }

    public void NewForm(Func<TItem, Task<Result>> onSave, TItem row) => ShowForm("新增", onSave, row);
    public void NewForm(Func<UploadInfo<TItem>, Task<Result>> onSave, TItem row) => ShowForm("新增", onSave, row);
    public void EditForm(Func<TItem, Task<Result>> onSave, TItem row) => ShowForm("编辑", onSave, row);
    public void EditForm(Func<UploadInfo<TItem>, Task<Result>> onSave, TItem row) => ShowForm("编辑", onSave, row);

    private void ShowForm(string action, Func<TItem, Task<Result>> onSave, TItem row)
    {
        var title = GetFormTitle(row);
        UI.ShowForm(new FormModel<TItem>(Form, this)
        {
            Title = $"{action}{title}",
            Data = row,
            OnSave = onSave
        });
    }

    private void ShowForm(string action, Func<UploadInfo<TItem>, Task<Result>> onSave, TItem row)
    {
        var title = GetFormTitle(row);
        UI.ShowForm(new FormModel<TItem>(Form, this)
        {
            Title = $"{action}{title}",
            Data = row,
            OnSaveFile = onSave
        });
    }

    public void ImportForm(ImportFormInfo info)
    {
        var option = new ModalOption { Title = $"导入{Name}" };
        option.Content = builder =>
        {
            builder.Component<Importer>()
                   .Set(c => c.Model, info)
                   .Set(c => c.OnSuccess, async () =>
                   {
                       option.OnClose?.Invoke();
                       await RefreshAsync();
                   })
                   .Build();
        };
        UI.ShowModal(option);
    }

    public void Delete(Func<List<TItem>, Task<Result>> action, TItem row)
    {
        UI.Confirm("确定要删除该记录？", async () =>
        {
            var result = await action?.Invoke([row]);
            UI.Result(result, async () => await RefreshAsync());
        });
    }

    public void DeleteM(Func<List<TItem>, Task<Result>> action) => Table?.SelectRows(action, "删除");

    private string GetFormTitle(TItem row)
    {
        var title = Name;
        if (FormTitle != null)
            title = FormTitle.Invoke(row);
        return title;
    }
}