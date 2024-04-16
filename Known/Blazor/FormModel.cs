using System.Linq.Expressions;
using Known.Extensions;
using Known.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Known.Blazor;

public class FormModel<TItem> : BaseModel where TItem : class, new()
{
    private bool isInitColumns = false;
    private List<ColumnInfo> columns = [];

    public FormModel(Context context, bool isAuto = true) : base(context)
    {
        Option = new FormOption();
        if (isAuto)
        {
            columns = typeof(TItem).GetProperties()
                                   .Select(p => new ColumnInfo(p))
                                   .Where(c => c.IsForm)
                                   .ToList();
        }
    }

    internal FormModel(BasePage page) : this(page.Context, false)
    {
        Page = page;
    }

    internal FormModel(TableModel<TItem> table) : this(table.Context, false)
    {
        Table = table;
        Page = table.Page;
        Option = table.Form;
        Type = table.FormType ?? Config.FormTypes.GetValueOrDefault($"{typeof(TItem).Name}Form");
        SetFormInfo(table.Module?.Form);
    }

    internal BasePage Page { get; }
    internal TableModel<TItem> Table { get; }
    internal string Action { get; set; }
    public string Title { get; set; }
    public string Class { get; set; } = "kui-form";
    public string ConfirmText { get; set; }
    public FormOption Option { get; }
    public bool Draggable { get; set; } = true;
    public bool Resizable { get; set; }
    public bool IsView { get; set; }
    public TItem Data { get; set; }
    public int? LabelSpan { get; set; }
    public int? WrapperSpan { get; set; }
    public List<FormRow<TItem>> Rows { get; } = [];
    public Dictionary<string, List<CodeInfo>> Codes { get; } = [];
    public Dictionary<string, FieldModel<TItem>> Fields { get; } = [];
    public Type Type { get; set; }
    public Func<bool> OnValidate { get; set; }
    public Func<Task> OnClose { get; set; }
    public Action<string> OnFieldChanged { get; set; }
    public Func<UploadInfo<TItem>, Task<Result>> OnSaveFile { get; set; }
    public Func<TItem, Task<Result>> OnSave { get; set; }
    public Func<bool> OnSaving { get; set; }
    public Action<TItem> OnSaved { get; set; }
    public Dictionary<string, List<IBrowserFile>> Files { get; } = [];

    internal FormViewType FormType { get; set; }
    
    internal List<CodeInfo> GetCodes(ColumnInfo column)
    {
        if (Codes.TryGetValue(column.Category, out List<CodeInfo> value))
            return value;

        return null;
    }

    public bool HasFile(string key)
    {
        if (Files == null)
            return false;

        if (!Files.TryGetValue(key, out List<IBrowserFile> value))
            return false;

        return value.Count > 0;
    }

    public void StateChanged()
    {
        foreach (var item in Fields)
        {
            item.Value.StateChanged();
        }
    }

    public FormRow<TItem> AddRow(Action<FormRow<TItem>> action = null)
    {
        var row = new FormRow<TItem>(this);
        action?.Invoke(row);
        Rows.Add(row);
        return row;
    }

    public ColumnBuilder<TItem> Field<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        var property = TypeHelper.Property(selector);
        var column = columns?.FirstOrDefault(c => c.Id == property.Name);
        return new ColumnBuilder<TItem>(column);
    }

    public void Initialize()
    {
        if (isInitColumns)
            return;

        isInitColumns = true;
        InitColumns();
    }

    public string GetFormTitle()
    {
        if (!string.IsNullOrWhiteSpace(Title))
            return Title;

        var action = Language?[$"Button.{Action}"];
        var title = Language?.GetString(Table.Module);
        if (Table.FormTitle != null)
            title = Table.FormTitle.Invoke(Data);
        return Language?["Title.FormAction"]?.Replace("{action}", action).Replace("{title}", title);
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

        if (OnSaving != null)
        {
            if (!OnSaving.Invoke())
                return;
        }

        if (string.IsNullOrWhiteSpace(ConfirmText))
        {
            await OnSaveAsync(isClose);
            return;
        }

        UI.Confirm(ConfirmText, async () => await OnSaveAsync(isClose));
    }

    private async Task OnSaveAsync(bool isClose)
    {
        try
        {
            var result = Result.Error("No save action.");
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
                result = await OnSaveFile.Invoke(info);
            }
            else if (OnSave != null)
            {
                result = await OnSave.Invoke(Data);
            }
            HandleResult(result, isClose);
        }
        catch (Exception ex)
        {
            UI.Error(ex.Message);
            Logger.Exception(ex);
        }
    }

    internal void HandleResult(Result result, bool isClose = false)
    {
        UI.Result(result, async () =>
        {
            Data = result.DataAs<TItem>();
            OnSaved?.Invoke(Data);
            if (result.IsClose || isClose)
                await CloseAsync();
            if (Table != null)
                await Table.PageRefreshAsync();
            else if (Page != null)
                await Page.RefreshAsync();
        });
    }

    internal void InitColumns()
    {
        if (columns == null || columns.Count == 0)
            return;

        Rows.Clear();
        var fields = columns.Where(c => c.IsVisible);
        var rowNos = fields.Select(c => c.Row).Distinct().OrderBy(r => r).ToList();
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

    internal void SetFormInfo(FormInfo info)
    {
        if (info == null)
            return;

        Option.LoadInfo(info);
        LabelSpan = info.LabelSpan;
        WrapperSpan = info.WrapperSpan;
        columns = GetFormColumns(info);
    }

    private static List<ColumnInfo> GetFormColumns(FormInfo form)
    {
        var columns = new List<ColumnInfo>();
        if (typeof(TItem) == typeof(Dictionary<string, object>))
        {
            columns = form?.Fields?.Select(f => new ColumnInfo(f)).ToList();
        }
        else
        {
            var allColumns = typeof(TItem).GetProperties().Select(p => new ColumnInfo(p)).ToList();
            foreach (var column in allColumns)
            {
                var info = form?.Fields?.FirstOrDefault(p => p.Id == column.Id);
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

    public FormRow<TItem> AddColumn<TValue>(string name, Expression<Func<TItem, TValue>> selector, Action<ColumnInfo> action = null)
    {
        return AddColumn(selector, c =>
        {
            c.Name = name;
            action?.Invoke(c);
        });
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

    internal void LoadInfo(FormInfo info)
    {
        if (info == null)
            return;

        Width = info.Width;
        Maximizable = info.Maximizable;
        DefaultMaximized = info.DefaultMaximized;
    }
}

enum FormViewType { View, Submit, Verify }