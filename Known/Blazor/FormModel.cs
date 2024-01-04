using System.Linq.Expressions;
using Known.Extensions;
using Known.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Known.Blazor;

public class FormModel<TItem> where TItem : class, new()
{
    private bool isInitColumns = false;
    private readonly List<ColumnInfo> columns = [];

    public FormModel(IUIService ui, bool isAuto = true)
    {
        if (isAuto)
        {
            columns = typeof(TItem).GetProperties()
                                   .Select(p => new ColumnInfo(p))
                                   .Where(c => c.IsForm)
                                   .ToList();
        }
        UI = ui;
        Option = new FormOption();
    }

    internal FormModel(IUIService ui, FormInfo info) : this(ui, false)
    {
        SetFormInfo(info);
        columns = info.Fields.Select(f => new ColumnInfo(f)).ToList();
    }

    internal FormModel(TableModel<TItem> table) : this(table.UI, false)
    {
        SetFormInfo(table.Module?.Form);
        Table = table;
        Page = table.Page;
        Option = table.Form;
        Type = Config.FormTypes.GetValueOrDefault($"{typeof(TItem).Name}Form");
        columns = GetFormColumns(table);
    }

    internal IUIService UI { get; }
	internal BasePage<TItem> Page { get; }
	internal TableModel<TItem> Table { get; }
    internal string Action { get; set; }

    public FormOption Option { get; }
    public Context Context => Page?.Context;
    public string Title => GetFormTitle(Data);
    public bool IsView { get; set; }
    public TItem Data { get; set; }
    public int? LabelSpan { get; set; }
    public int? WrapperSpan { get; set; }
    public List<FormRow<TItem>> Rows { get; } = [];
    public Dictionary<string, List<CodeInfo>> Codes { get; } = [];
    public Dictionary<string, FieldModel<TItem>> Fields { get; } = [];
    public Type Type { get; internal set; }
    public Func<bool> OnValidate { get; set; }
    public Func<Task> OnClose { get; set; }
    public Action<string> OnFieldChanged { get; set; }

    internal FormType FormType { get; set; }
    internal Func<TItem, Task<Result>> OnSave { get; set; }
    internal Func<UploadInfo<TItem>, Task<Result>> OnSaveFile { get; set; }
    internal Dictionary<string, List<IBrowserFile>> Files { get; } = [];

    internal List<CodeInfo> GetCodes(ColumnInfo column)
    {
        if (Codes.TryGetValue(column.Category, out List<CodeInfo> value))
            return value;

        return null;
    }

    public void StateChanged()
    {
        foreach (var item in Fields)
        {
            item.Value.StateChanged();
        }
    }

    public FormRow<TItem> AddRow()
    {
        var row = new FormRow<TItem>(this);
        Rows.Add(row);
        return row;
    }

    public ColumnBuilder<TItem> Column<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        var property = TypeHelper.Property(selector);
        var column = columns?.FirstOrDefault(c => c.Id == property.Name);
        return new ColumnBuilder<TItem>(column);
    }

    internal void Initialize()
    {
        if (isInitColumns)
            return;

        isInitColumns = true;
        if (columns != null && columns.Count > 0)
        {
            Rows.Clear();
            var fields = columns.Where(c => c.IsVisible);
            var rowNos = fields.Select(c => c.Row).Distinct().ToList();
            if (rowNos.Count == 1)
            {
                foreach (var item in fields)
                {
                    AddRow().AddColumn(item);
                }
            }
            else
            {
                foreach (var rowNo in rowNos)
                {
                    var infos = fields.Where(c => c.Row == rowNo).OrderBy(c => c.Column).ToArray();
                    AddRow().AddColumn(infos);
                }
            }
        }
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
        HandleResult(result, isClose);
    }

    internal void HandleResult(Result result, bool isClose = false)
    {
        UI.Result(result, async () =>
        {
            if (result.IsClose || isClose)
                await CloseAsync();
            await Page.RefreshAsync();
        });
    }

    private string GetFormTitle(TItem row)
    {
        var title = Table.Page?.Name;
        if (Table.FormTitle != null)
            title = Table.FormTitle.Invoke(row);
        return Page?.Language["Title.FormAction"].Replace("{action}", Action).Replace("{title}", title);
    }

    private void SetFormInfo(FormInfo info)
    {
        if (info == null)
            return;

        LabelSpan = info.LabelSpan;
        WrapperSpan = info.WrapperSpan;
    }

    private static List<ColumnInfo> GetFormColumns(TableModel<TItem> table)
    {
        var columns = new List<ColumnInfo>();
        var isDictionary = typeof(TItem) == typeof(Dictionary<string, object>);
        if (isDictionary)
        {
            columns = table.Module?.Form?.Fields?.Select(f => new ColumnInfo(f)).ToList();
        }
        else
        {
            var allColumns = typeof(TItem).GetProperties().Select(p => new ColumnInfo(p)).ToList();
            foreach (var column in allColumns)
            {
                var info = table.Module?.Form?.Fields?.FirstOrDefault(p => p.Id == column.Id);
                if (info != null)
                {
                    column.SetFormFieldInfo(info);
                    columns.Add(column);
                }
            }
        }
        return columns;
    }
}

public class FormRow<TItem> where TItem : class, new()
{
    internal FormRow(FormModel<TItem> form)
    {
        Form = form;
    }

    public FormModel<TItem> Form { get; }
    public List<FieldModel<TItem>> Fields { get; } = [];

    public FormRow<TItem> AddColumn(string id, string text) => AddColumn(id, b => b.Text(text));
    public FormRow<TItem> AddColumn(string id, RenderFragment template) => AddColumn(new ColumnInfo(id, template));

    public FormRow<TItem> AddColumn<TValue>(Expression<Func<TItem, TValue>> selector, Action<ColumnInfo> action = null)
    {
        var property = TypeHelper.Property(selector);
        var column = new ColumnInfo(property);
        action?.Invoke(column);
        return AddColumn(column);
    }

    public FormRow<TItem> AddColumn(params ColumnInfo[] columns)
    {
        foreach (var item in columns)
        {
            item.IsForm = true;
            Fields.Add(new FieldModel<TItem>(Form, item));
        }
        return this;
    }
}

public class FormOption
{
    public double? Width { get; set; }
    public bool Maximizable { get; set; }
    public bool DefaultMaximized { get; set; }
    public bool NoFooter { get; set; }
}

enum FormType { View, Submit, Verify }