using System.Linq.Expressions;
using System.Reflection;
using Known.Extensions;
using Known.Helpers;
using Microsoft.AspNetCore.Components;

namespace Known.Razor;

public class PageModel<TItem>
{
    public List<ActionInfo> Tools { get; set; }
    public Action<ActionInfo> OnToolClick { get; set; }
    public TableModel<TItem> Table { get; set; }
    public TreeModel Tree { get; set; }
}

public class TreeModel
{
    public List<MenuItem> Data { get; set; }
}

public class TableModel<TItem>
{
    private readonly IUIService UI;

    internal TableModel(IUIService ui, List<ColumnInfo> columns, List<ActionInfo> actions)
    {
        UI = ui;
        Actions = actions;
        Columns = TypeHelper.GetColumnAttributes(typeof(TItem))
                            .Where(c => HasColumn(columns, c.Property.Name))
                            .ToList();

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

    public bool ShowCheckBox { get; set; }
    public string PageName { get; set; }
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

    public void ShowForm(Func<dynamic, Task<Result>> action, TItem row)
    {
        var actionName = row == null ? "新增" : "编辑";
        UI.ShowForm<TItem>(new FormModel<TItem>(UI, Columns)
        {
            Title = $"{actionName}{PageName}",
            Data = row,
            Action = action
        });
    }

    public void ShowImportForm()
    {
        UI.Info("暂未实现导入表单！");
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
                    await UI.Result(result, async () => await RefreshAsync());
                });
            }
            else
            {
                var result = await action?.Invoke(row);
                await UI.Result(result, async () => await RefreshAsync());
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
        if (rows.Count == 0 || rows.Count > 1)
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
                    await UI.Result(result, async () => await RefreshAsync());
                });
            }
            else
            {
                var result = await action?.Invoke(rows);
                await UI.Result(result, async () => await RefreshAsync());
            }
        });
    }

    public void Delete(Func<List<TItem>, Task<Result>> action, TItem row)
    {
        UI.Confirm("确定要删除该记录？", async () =>
        {
            var result = await action?.Invoke([row]);
            await UI.Result(result, async () => await RefreshAsync());
        });
    }

    public void DeleteM(Func<List<TItem>, Task<Result>> action) => SelectRows(action, "删除");

    private static bool HasColumn(List<ColumnInfo> columns, string id)
    {
        if (columns == null || columns.Count == 0)
            return true;

        return columns.Any(c => c.Id == id);
    }
}

public class FormModel<TItem>
{
    internal FormModel(IUIService ui, List<ColumnAttribute> columns)
    {
        Fields = columns.Where(c => c.IsForm).Select(c => new FieldModel<TItem>(ui, this, c));
    }

    public IEnumerable<FieldModel<TItem>> Fields { get; }
    public string Title { get; set; }
    public TItem Data { get; set; }
    public Type Type { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public Func<dynamic, Task<Result>> Action { get; set; }
}

public class FieldModel<TItem>
{
    private readonly IUIService UI;
    private FormModel<TItem> _form;
    private RenderFragment _inputTemplate;

    internal FieldModel(IUIService ui, FormModel<TItem> form, ColumnAttribute column)
    {
        UI = ui;
        _form = form;
        Column = column;
    }

    public ColumnAttribute Column { get; }
    public Action<TItem, object> ValueChanged { get; set; }

    public object Value
    {
        get => Column.Property.GetValue(_form.Data);
        set
        {
            if (Column.Property.SetMethod is not null && !Equals(Value, value))
            {
                Column.Property.SetValue(_form.Data, value);
                ValueChanged?.Invoke(_form.Data, value);
            }
        }
    }

    public RenderFragment InputTemplate
    {
        get
        {
            return _inputTemplate ??= builder =>
            {
                var inputType = UI.GetInputType(Column);
                builder.OpenComponent(0, inputType);
                //builder.AddMultipleAttributes(1, InputComponentAttributes);
                builder.CloseComponent();
            };
        }
    }
}