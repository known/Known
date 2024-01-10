using System.Linq.Expressions;
using Known.Entities;
using Known.Extensions;
using Known.Helpers;
using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

public class TableModel<TItem> where TItem : class, new()
{
    internal TableModel()
    {
        AllColumns = GetAllColumns();
        Columns = AllColumns?.Where(c => !string.IsNullOrWhiteSpace(c.Name))?.ToList();
        InitQueryColumns();
    }

    internal TableModel(IUIService ui, Context context, SysModule module) : this(ui, module?.Page)
    {
        Context = context;
        Module = module;
    }

    internal TableModel(IUIService ui, PageInfo info)
    {
        UI = ui;
        SetPageInfo(info);
        Columns = AllColumns;
        InitQueryColumns();
    }

	internal TableModel(BasePage<TItem> page)
	{
        Page = page;
        Context = page.Context;
		UI = page.UI;
        Module = page.Module;
        SetPageInfo(Module?.Page, page);
        if (AllColumns != null && AllColumns.Count > 0)
        {
            foreach (var column in AllColumns)
            {
                var info = page.Columns?.FirstOrDefault(p => p.Id == column.Id);
                if (info != null)
                {
                    column.SetPageColumnInfo(info);
                    Columns.Add(column);
                }
            }
        }
        InitQueryColumns();
    }

    internal Context Context { get; }
    internal Language Language => Context?.Language;
    internal IUIService UI { get; }
    internal List<ColumnInfo> AllColumns { get; private set; }
    internal BasePage<TItem> Page { get; }
    internal SysModule Module { get; }

    public bool ShowCheckBox { get; private set; }
    public bool ShowPager { get; set; }
    public string FixedWidth { get; set; }
    public string FixedHeight { get; set; }
    public FormOption Form { get; } = new();
    public Func<TItem, string> FormTitle { get; set; }
    public ToolbarModel Toolbar { get; } = new();
    public List<ColumnInfo> Columns { get; } = [];
    public List<ColumnInfo> QueryColumns { get; } = [];
    public Dictionary<string, QueryInfo> QueryData { get; } = [];
    public PagingCriteria Criteria { get; } = new();
    public PagingResult<TItem> Result { get; set; } = new();
    public List<ActionInfo> Actions { get; private set; }
    public IEnumerable<TItem> SelectedRows { get; set; }
    public Dictionary<string, RenderFragment<TItem>> Templates { get; } = [];
    public Func<TItem, object> RowKey { get; set; }
    public Func<TItem, List<ActionInfo>> RowActions { get; set; }
    public Func<PagingCriteria, Task<PagingResult<TItem>>> OnQuery { get; set; }
    public Action<ActionInfo, TItem> OnAction { get; set; }
    public Func<Task> OnRefresh { get; set; }

    public ColumnBuilder<TItem> Column<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        var property = TypeHelper.Property(selector);
        var column = Columns?.FirstOrDefault(c => c.Id == property.Name);
        return new ColumnBuilder<TItem>(column, this);
    }

    public void AddQueryColumn(Expression<Func<TItem, object>> selector)
    {
        var property = TypeHelper.Property(selector);
        if (QueryColumns.Exists(c => c.Id == property.Name))
            return;

        var column = new ColumnInfo(property);
        QueryColumns.Add(column);
        QueryData[property.Name] = new QueryInfo(column);
    }

    public Task RefreshAsync()
    {
        if (OnRefresh == null)
            return Task.CompletedTask;

        return OnRefresh.Invoke();
    }

	public void ViewForm(TItem row) => ViewForm(FormType.View, row);

    internal void ViewForm(FormType type, TItem row)
    {
        UI.ShowForm(new FormModel<TItem>(this)
        {
            Context = Context,
            FormType = type,
            IsView = true,
            Action = $"{type}",
            Data = row
        });
    }

    public void NewForm(Func<TItem, Task<Result>> onSave, TItem row) => ShowForm("New", onSave, row);
    public void NewForm(Func<UploadInfo<TItem>, Task<Result>> onSave, TItem row) => ShowForm("New", onSave, row);
    public void EditForm(Func<TItem, Task<Result>> onSave, TItem row) => ShowForm("Edit", onSave, row);
    public void EditForm(Func<UploadInfo<TItem>, Task<Result>> onSave, TItem row) => ShowForm("Edit", onSave, row);
    public void DeleteM(Func<List<TItem>, Task<Result>> action) => SelectRows(action, "Delete");

    public void Delete(Func<List<TItem>, Task<Result>> action, TItem row)
    {
        UI.Confirm(Language?["Tip.ConfirmDeleteRecord"], async () =>
        {
            var result = await action?.Invoke([row]);
            UI.Result(result, async () => await RefreshAsync());
        });
    }

    public void SelectRow(Action<TItem> action)
    {
        if (SelectedRows == null)
        {
            UI.Warning(Language?.SelectOne);
            return;
        }

        var rows = SelectedRows.ToList();
        if (rows.Count == 0 || rows.Count > 1)
        {
            UI.Warning(Language?.SelectOne);
            return;
        }

        action?.Invoke(rows[0]);
    }

    public void SelectRow(Func<TItem, Task<Result>> action, string confirmText = null)
    {
        SelectRow(async row =>
        {
            if (!string.IsNullOrWhiteSpace(confirmText))
            {
                UI.Confirm(GetConfirmText(confirmText), async () =>
                {
                    var result = await action?.Invoke(row);
                    UI.Result(result, async () => await Page.RefreshAsync());
                });
            }
            else
            {
                var result = await action?.Invoke(row);
                UI.Result(result, async () => await Page.RefreshAsync());
            }
        });
    }

    public void SelectRows(Action<List<TItem>> action)
    {
        if (SelectedRows == null)
        {
            UI.Warning(Language?.SelectOneAtLeast);
            return;
        }

        var rows = SelectedRows.ToList();
        if (rows.Count == 0)
        {
            UI.Warning(Language?.SelectOneAtLeast);
            return;
        }

        action?.Invoke(rows);
    }

    public void SelectRows(Func<List<TItem>, Task<Result>> action, string confirmText = null)
    {
        SelectRows(async rows =>
        {
            if (!string.IsNullOrWhiteSpace(confirmText))
            {
                UI.Confirm(GetConfirmText(confirmText), async () =>
                {
                    var result = await action?.Invoke(rows);
                    UI.Result(result, async () => await Page.RefreshAsync());
                });
            }
            else
            {
                var result = await action?.Invoke(rows);
                UI.Result(result, async () => await Page.RefreshAsync());
            }
        });
    }

    private void ShowForm(string action, Func<TItem, Task<Result>> onSave, TItem row)
    {
        UI.ShowForm(new FormModel<TItem>(this) { Context = Context, Action = action, Data = row, OnSave = onSave });
    }

    private void ShowForm(string action, Func<UploadInfo<TItem>, Task<Result>> onSave, TItem row)
    {
        UI.ShowForm(new FormModel<TItem>(this) { Context = Context, Action = action, Data = row, OnSaveFile = onSave });
    }

    private static List<ColumnInfo> GetAllColumns()
    {
        return typeof(TItem).GetProperties().Select(p => new ColumnInfo(p)).ToList();
    }

    private void SetPageInfo(PageInfo info, BasePage<TItem> page = null)
    {
        if (info == null)
            return;

        FixedWidth = info.FixedWidth;
        FixedHeight = info.FixedHeight;
        ShowPager = info.ShowPager;

        if (page == null)
        {
            Toolbar.Items = info.Tools?.Select(t => new ActionInfo(t)).ToList();
            Actions = info.Actions?.Select(a => new ActionInfo(a)).ToList();
            AllColumns = info.Columns.Select(c => new ColumnInfo(c)).ToList();
        }
        else
        {
            Toolbar.Items = info.Tools?.Where(t => page.Tools.Contains(t)).Select(t => new ActionInfo(t)).ToList();
            Actions = info.Actions?.Where(a => page.Actions.Contains(a)).Select(a => new ActionInfo(a)).ToList();
            AllColumns = GetAllColumns();
        }

        ShowCheckBox = Toolbar.Items != null && Toolbar.Items.Count > 0;
    }

    private void InitQueryColumns()
    {
        if (Columns != null && Columns.Count > 0)
            QueryColumns.AddRange(Columns.Where(c => c.IsQuery));

        if (QueryColumns != null && QueryColumns.Count > 0)
        {
            foreach (var item in QueryColumns)
            {
                QueryData[item.Id] = new QueryInfo(item);
            }
        }
    }

    private string GetConfirmText(string text) => Language?["Tip.ConfirmRecordName"]?.Replace("{text}", text);
}