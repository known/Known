using System.Linq.Expressions;
using System.Reflection;
using Known.Extensions;
using Known.Helpers;
using Microsoft.AspNetCore.Components;

namespace Known.Razor;

public class TableModel<TItem> where TItem : class, new()
{
    private readonly List<ColumnAttribute> allColumns;

    internal TableModel(IUIService ui, List<ColumnInfo> columns, List<ActionInfo> actions)
    {
        allColumns = TypeHelper.GetColumnAttributes(typeof(TItem));
        UI = ui;
        Actions = actions;
        Columns = allColumns.Where(c => HasColumn(columns, c.Property.Name)).ToList();

        if (Columns != null && Columns.Count > 0)
        {
            QueryColumns = Columns.Where(c => c.IsQuery).ToList();
            if (QueryColumns != null && QueryColumns.Count > 0)
            {
                foreach (var item in QueryColumns)
                {
                    QueryData[item.Property.Name] = new QueryInfo(item);
                }
            }
        }
    }

    internal IUIService UI { get; }
    public string Name { get; set; }
    public bool ShowCheckBox { get; set; }
    public List<ColumnAttribute> Columns { get; }
    public List<ColumnAttribute> QueryColumns { get; } = [];
    public Dictionary<string, QueryInfo> QueryData { get; } = [];
    public PagingCriteria Criteria { get; } = new();
    public PagingResult<TItem> Result { get; set; } = new();
    public List<ActionInfo> Actions { get; }
    public IEnumerable<TItem> SelectedRows { get; set; }
    public Dictionary<string, RenderFragment<TItem>> Templates { get; set; }
    public Func<PagingCriteria, Task<PagingResult<TItem>>> OnQuery { get; set; }
    public Action<ActionInfo, TItem> OnAction { get; set; }
    public Func<Task> OnRefresh { get; set; }
    public Func<TItem, string> FormTitle { get; set; }

    public ColumnBuilder<TItem> Column<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        var property = TypeHelper.Property(selector);
        var column = Columns?.FirstOrDefault(c => c.Property.Name == property.Name);
        return new ColumnBuilder<TItem>(this, property.Name, column);
    }

    public void AddQueryColumn(Expression<Func<TItem, object>> selector)
    {
        var property = TypeHelper.Property(selector);
        if (QueryColumns.Exists(c => c.Property.Name == property.Name))
            return;

        var attr = property.GetCustomAttribute<ColumnAttribute>();
        if (attr != null)
        {
            attr.Property = property;
            QueryColumns.Add(attr);
            QueryData[property.Name] = new QueryInfo(attr);
        }
    }

    public Task RefreshAsync() => OnRefresh?.Invoke();

    public void ViewForm(TItem row)
    {
        var title = GetFormTitle(row);
        UI.ShowForm<TItem>(new FormModel<TItem>(this, allColumns)
        {
            IsView = true,
            Title = $"查看{title}",
            Data = row
        });
    }

    public void NewForm(Func<TItem, Task<Result>> onSave, TItem row) => ShowForm("新增", onSave, row);
    public void EditForm(Func<TItem, Task<Result>> onSave, TItem row) => ShowForm("编辑", onSave, row);

    private void ShowForm(string action, Func<TItem, Task<Result>> onSave, TItem row)
    {
        var title = GetFormTitle(row);
        UI.ShowForm<TItem>(new FormModel<TItem>(this, allColumns)
        {
            Title = $"{action}{title}",
            Data = row,
            OnSave = onSave
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
                    UI.Result(result, async () => await RefreshAsync());
                });
            }
            else
            {
                var result = await action?.Invoke(row);
                UI.Result(result, async () => await RefreshAsync());
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
                    UI.Result(result, async () => await RefreshAsync());
                });
            }
            else
            {
                var result = await action?.Invoke(rows);
                UI.Result(result, async () => await RefreshAsync());
            }
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

    public void DeleteM(Func<List<TItem>, Task<Result>> action) => SelectRows(action, "删除");

    private static bool HasColumn(List<ColumnInfo> columns, string id)
    {
        if (columns == null || columns.Count == 0)
            return true;

        return columns.Any(c => c.Id == id);
    }

    private string GetFormTitle(TItem row)
    {
        var title = Name;
        if (FormTitle != null)
            title = FormTitle.Invoke(row);
        return title;
    }
}