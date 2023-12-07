using Known.Extensions;
using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

public class PageModel
{
    public PageType Type { get; set; }
    public int[] Spans { get; set; }
	public List<RenderFragment> Contents { get; set; }
}

public class PageModel<TItem> : PageModel where TItem : class, new()
{
    internal PageModel(BasePage<TItem> page)
    {
        UI = page.UI;
        Page = page;
        Name = page.Name;
        Tools = page.Tools;
        Table = new TableModel<TItem>(page);
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

    public void NewForm(Func<TItem, Task<Result>> onSave, TItem row) => ShowForm("新增", onSave, row);
    public void EditForm(Func<TItem, Task<Result>> onSave, TItem row) => ShowForm("编辑", onSave, row);

    private void ShowForm(string action, Func<TItem, Task<Result>> onSave, TItem row)
    {
        var title = GetFormTitle(row);
        UI.ShowForm(new FormModel<TItem>(Page, Form)
        {
            Title = $"{action}{title}",
            Data = row,
            OnSave = onSave
        });
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