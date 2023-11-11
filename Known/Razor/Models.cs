using System.Linq.Expressions;
using System.Reflection;
using Known.Extensions;
using Known.Helpers;
using Microsoft.AspNetCore.Components;

namespace Known.Razor;

public class PageModel<TItem> where TItem : class, new()
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

public class TableModel<TItem> where TItem : class, new()
{
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

    internal IUIService UI { get; }
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
        UI.ShowForm<TItem>(new FormModel<TItem>(this, Columns)
        {
            IsView = true,
            Title = $"查看{title}",
            Data = row
        });
    }

    public void ShowForm(Func<TItem, Task<Result>> onSave, TItem row)
    {
        var actionName = row == null ? "新增" : "编辑";
        var title = GetFormTitle(row);
        UI.ShowForm<TItem>(new FormModel<TItem>(this, Columns)
        {
            Title = $"{actionName}{title}",
            Data = row,
            OnSave = onSave
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
        var title = PageName;
        if (FormTitle != null)
            title = FormTitle.Invoke(row);
        return title;
    }
}

public class FormModel<TItem> where TItem : class, new()
{
    internal FormModel(TableModel<TItem> table, List<ColumnAttribute> columns)
    {
        Table = table;
        Fields = columns.Where(c => c.IsForm).Select(c => new FieldModel<TItem>(table.UI, this, c));
    }

    public TableModel<TItem> Table { get; }
    public IEnumerable<FieldModel<TItem>> Fields { get; }
    public bool IsView { get; set; }
    public string Title { get; set; }
    public TItem Data { get; set; }
    public Type Type { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public Func<bool> OnValidate { get; set; }
    public Func<Task> OnClose { get; set; }
    public Func<TItem, Task<Result>> OnSave { get; set; }

    public async Task SaveAsync()
    {
        if (OnValidate != null)
        {
            if (!OnValidate.Invoke()) return;
        }

        var result = await OnSave?.Invoke(Data);
        Table.UI.Result(result, async () =>
        {
            if (result.IsClose)
                await OnClose?.Invoke();
            await Table.RefreshAsync();
        });
    }
}

public class FieldModel<TItem> where TItem : class, new()
{
    private readonly IUIService UI;
    private FormModel<TItem> _form;
    private RenderFragment _inputTemplate;

    internal FieldModel(IUIService ui, FormModel<TItem> form, ColumnAttribute column)
    {
        UI = ui;
        _form = form;
        Column = column;
        Data = form.Data;
    }

    public ColumnAttribute Column { get; }
    public TItem Data { get; }
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
                builder.AddMultipleAttributes(1, InputAttributes);
                builder.CloseComponent();
            };
        }
    }

    private IDictionary<string, object> InputAttributes
    {
        get
        {
            var expression = InputExpression.Create(this);
            var attributes = new Dictionary<string, object>
            {
                { "id", Column.Property.Name },
                { "Value", Value },
                { "ValueExpression", expression.ValueExpression },
                { "autofocus", true },
                { "required", Column.Required },
                { "placeholder", Column.Placeholder },
            };
            if (_form.IsView)
            {
                attributes["disabled"] = "true";
                attributes["readonly"] = "true";
            }
            else
            {
                attributes["ValueChanged"] = expression.ValueChanged;
            }
            return attributes;
        }
    }
}

record InputExpression(LambdaExpression ValueExpression, object ValueChanged)
{
    private static readonly MethodInfo EventCallbackFactory = GetEventCallbackFactory();

    private static MethodInfo GetEventCallbackFactory()
    {
        return typeof(EventCallbackFactory)
            .GetMethods()
            .Single(m =>
            {
                if (m.Name != "Create" || !m.IsPublic || m.IsStatic || !m.IsGenericMethod)
                    return false;

                var generic = m.GetGenericArguments();
                if (generic.Length != 1)
                    return false;

                var args = m.GetParameters();
                return args.Length == 2
                       && args[0].ParameterType == typeof(object)
                       && args[1].ParameterType.IsGenericType
                       && args[1].ParameterType.GetGenericTypeDefinition() == typeof(Action<>);
            });
    }

    public static InputExpression Create<TItem>(FieldModel<TItem> model) where TItem : class, new()
    {
        // () => Owner.Property
        var property = model.Column.Property;
        var access = Expression.Property(
            Expression.Constant(model.Data, typeof(TItem)),
            property);
        var lambda = Expression.Lambda(typeof(Func<>).MakeGenericType(property.PropertyType), access);

        // Create(object receiver, Action<object>) callback
        var method = EventCallbackFactory.MakeGenericMethod(property.PropertyType);

        // value => Field.Value = value;
        var changeHandlerParameter = Expression.Parameter(property.PropertyType);

        var body = Expression.Assign(
            Expression.Property(Expression.Constant(model), nameof(model.Value)),
            Expression.Convert(changeHandlerParameter, typeof(object)));

        var changeHandlerLambda = Expression.Lambda(
            typeof(Action<>).MakeGenericType(property.PropertyType),
            body,
            changeHandlerParameter);

        var changeHandler = method.Invoke(
            EventCallback.Factory,
            new object[] { model, changeHandlerLambda.Compile() });

        return new InputExpression(lambda, changeHandler);
    }
}