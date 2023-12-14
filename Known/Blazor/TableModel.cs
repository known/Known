using System.Linq.Expressions;
using System.Reflection;
using Known.Extensions;
using Known.Helpers;
using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

public class TableModel<TItem> where TItem : class, new()
{
    internal TableModel()
    {
        AllColumns = TypeHelper.GetColumnAttributes(typeof(TItem)).Select(a => new ColumnInfo(a)).ToList();
        Columns = AllColumns;
        InitQueryColumns();
    }

	internal TableModel(BasePage<TItem> page)
	{
        Page = page;
		UI = page.UI;
		Actions = page.Actions;
		ShowCheckBox = page.Tools != null && page.Tools.Count > 0;
		ShowPager = true;
        AllColumns = page.AllColumns;
		Columns = AllColumns.Where(c => c.IsGrid && HasColumn(page.Columns, c.Property.Name)).ToList();
		InitQueryColumns();
	}

    internal IUIService UI { get; }
    internal List<ColumnInfo> AllColumns { get; }
	internal BasePage<TItem> Page { get; }

    public bool ShowCheckBox { get; }
    public bool ShowPager { get; set; }
    public List<ColumnInfo> Columns { get; }
    public List<ColumnInfo> QueryColumns { get; } = [];
    public Dictionary<string, QueryInfo> QueryData { get; } = [];
    public PagingCriteria Criteria { get; } = new();
    public PagingResult<TItem> Result { get; set; } = new();
    public List<ActionInfo> Actions { get; }
    public IEnumerable<TItem> SelectedRows { get; set; }
    public Dictionary<string, RenderFragment<TItem>> Templates { get; } = [];
    public Func<TItem, object> RowKey { get; set; }
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

        var attr = property.GetCustomAttribute<ColumnAttribute>();
        if (attr != null)
        {
            attr.Property = property;
            var column = new ColumnInfo(attr);
            QueryColumns.Add(column);
            QueryData[property.Name] = new QueryInfo(column);
        }
    }

    public Task RefreshAsync()
    {
        if (OnRefresh == null)
            return Task.CompletedTask;

        return OnRefresh.Invoke();
    }

	public void ViewForm(TItem row) => ViewForm(FormType.View, row);
	public virtual void ViewForm(FormType type, TItem row) { }

	public void SelectRow(Action<TItem> action)
    {
        if (SelectedRows == null)
        {
            UI.Warning(Language.SelectOne);
            return;
        }

        var rows = SelectedRows.ToList();
        if (rows.Count == 0 || rows.Count > 1)
        {
            UI.Warning(Language.SelectOne);
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
                UI.Confirm($"确定要{confirmText}选中的记录？", async () =>
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
            UI.Warning(Language.SelectOneAtLeast);
            return;
        }

        var rows = SelectedRows.ToList();
        if (rows.Count == 0)
        {
            UI.Warning(Language.SelectOneAtLeast);
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
                UI.Confirm($"确定要{confirmText}选中的记录？", async () =>
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

	private static bool HasColumn(List<ColumnInfo> columns, string id)
    {
        if (columns == null || columns.Count == 0)
            return true;

        return columns.Any(c => c.Id == id);
    }

    private void InitQueryColumns()
    {
        if (Columns != null && Columns.Count > 0)
            QueryColumns.AddRange(Columns.Where(c => c.IsQuery));

        if (QueryColumns != null && QueryColumns.Count > 0)
        {
            foreach (var item in QueryColumns)
            {
                QueryData[item.Property.Name] = new QueryInfo(item);
            }
        }
    }
}

public class TablePageModel<TItem> : TableModel<TItem> where TItem : class, new()
{
    internal TablePageModel(BasePage<TItem> page) : base(page)
    {
        Toolbar = new ToolbarModel { Items = page.Tools };
        Form = new FormOption();
    }

	public string Name { get; }
    public ToolbarModel Toolbar { get; }
	public FormOption Form { get; }
    public Func<TItem, string> FormTitle { get; set; }

	public void NewForm(Func<TItem, Task<Result>> onSave, TItem row) => ShowForm("新增", onSave, row);
	public void NewForm(Func<UploadInfo<TItem>, Task<Result>> onSave, TItem row) => ShowForm("新增", onSave, row);
	public void EditForm(Func<TItem, Task<Result>> onSave, TItem row) => ShowForm("编辑", onSave, row);
	public void EditForm(Func<UploadInfo<TItem>, Task<Result>> onSave, TItem row) => ShowForm("编辑", onSave, row);

	public override void ViewForm(FormType type, TItem row)
	{
		var actionName = type.GetDescription();
		var title = GetFormTitle(row);
		UI.ShowForm(new FormModel<TItem>(this, Form)
		{
			FormType = type,
			IsView = true,
			Title = $"{actionName}{title}",
			Data = row
		});
	}

	private void ShowForm(string action, Func<TItem, Task<Result>> onSave, TItem row)
	{
		var title = GetFormTitle(row);
		UI.ShowForm(new FormModel<TItem>(this, Form)
		{
			Title = $"{action}{title}",
			Data = row,
			OnSave = onSave
		});
	}

	private void ShowForm(string action, Func<UploadInfo<TItem>, Task<Result>> onSave, TItem row)
	{
		var title = GetFormTitle(row);
		UI.ShowForm(new FormModel<TItem>(this, Form)
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
					   await Page.RefreshAsync();
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
			UI.Result(result, async () => await Page.RefreshAsync());
		});
	}

	public void DeleteM(Func<List<TItem>, Task<Result>> action) => SelectRows(action, "删除");

	private string GetFormTitle(TItem row)
	{
		var title = Name;
		if (FormTitle != null)
			title = FormTitle.Invoke(row);
		return title;
	}
}