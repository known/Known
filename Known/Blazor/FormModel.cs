using System.ComponentModel;
using System.Linq.Expressions;
using Known.Extensions;
using Known.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Known.Blazor;

public class FormModel<TItem> where TItem : class, new()
{
    private readonly List<ColumnInfo> columns;

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

    internal FormModel(TableModel<TItem> table, FormOption option)
    {
        Table = table;
        if (table.AllColumns != null)
            columns = table.AllColumns.Where(c => c.IsForm).ToList();

        UI = table.UI;
        Page = table.Page;
        Option = option;
        Type = Config.FormTypes.GetValueOrDefault($"{typeof(TItem).Name}Form");
    }

    internal IUIService UI { get; }
	internal BasePage<TItem> Page { get; }
	internal TableModel<TItem> Table { get; }

    public FormOption Option { get; }
    public string Title { get; internal set; }
    public bool IsView { get; set; }
    public TItem Data { get; set; }
    public int? LabelSpan { get; set; }
    public int? WrapperSpan { get; set; }
    public List<FormRow<TItem>> Rows { get; } = [];
    public Dictionary<string, List<CodeInfo>> Codes { get; } = [];
    public Dictionary<string, List<IBrowserFile>> Files { get; } = [];
    public Type Type { get; internal set; }
    public Func<bool> OnValidate { get; set; }
    public Func<Task> OnClose { get; set; }

    internal FormType FormType { get; set; }
    internal Func<TItem, Task<Result>> OnSave { get; set; }
    internal Func<UploadInfo<TItem>, Task<Result>> OnSaveFile { get; set; }

    internal List<CodeInfo> GetCodes(ColumnInfo column)
    {
        if (Codes.TryGetValue(column.Category, out List<CodeInfo> value))
            return value;

        return null;
    }

    public FormRow<TItem> AddRow()
    {
        var row = new FormRow<TItem>(this);
        Rows.Add(row);
        return row;
    }

    public ColumnBuilder<TItem> Column<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        if (Table != null)
            return Table.Column(selector);

        var property = TypeHelper.Property(selector);
        var column = columns?.FirstOrDefault(c => c.Id == property.Name);
        return new ColumnBuilder<TItem>(column);
    }

    public void Initialize()
    {
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
        UI.Result(result, async () =>
        {
            if (result.IsClose || isClose)
                await CloseAsync();
            await Page.RefreshAsync();
        });
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

    public FormRow<TItem> AddColumn(string label, string text) => AddColumn(label, b => b.Text(text));

    public FormRow<TItem> AddColumn(string label, RenderFragment template)
    {
        var column = new ColumnInfo() { Name = label, Template = template };
        return AddColumn(column);
    }

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

public enum FormType
{
    [Description("查看")] View,
    [Description("提交")] Submit,
    [Description("审核")] Verify
}