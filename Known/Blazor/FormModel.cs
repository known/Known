namespace Known.Blazor;

public class FormModel<TItem> : BaseModel where TItem : class, new()
{
    private bool isInitColumns = false;
    private List<ColumnInfo> columns = [];

    public FormModel(Context context, bool isAuto = false) : base(context)
    {
        if (isAuto)
        {
            columns = TypeHelper.Properties(typeof(TItem))
                                .Select(p => new ColumnInfo(p))
                                .Where(c => c.IsForm)
                                .ToList();
        }
        Data = new TItem();
    }

    internal FormModel(BasePage page) : this(page.Context)
    {
        Page = page;
    }

    internal FormModel(TableModel<TItem> table) : this(table.Context)
    {
        Table = table;
        Page = table.Page;
        Type = table.FormType ?? Config.FormTypes.GetValueOrDefault($"{typeof(TItem).Name}Form");
        SetFormInfo(table.Module?.Form);
    }

    internal BasePage Page { get; }
    internal TableModel<TItem> Table { get; }
    internal string Action { get; set; }
    public string Title { get; set; }
    public string Class { get; set; }
    public string ConfirmText { get; set; }
    public Func<string> OnConfirmText { get; set; }
    public double? Width { get; set; }
    public bool SmallLabel { get; set; }
    public bool Maximizable { get; set; }
    public bool DefaultMaximized { get; set; }
    public bool NoFooter { get; set; }
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
    public RenderFragment Footer { get; set; }
    public Func<bool> OnValidate { get; set; }
    public Func<Task> OnClose { get; set; }
    public Action OnClosed { get; set; }
    public Action<string> OnFieldChanged { get; set; }
    public Func<UploadInfo<TItem>, Task<Result>> OnSaveFile { get; set; }
    public Func<TItem, Task<Result>> OnSave { get; set; }
    public Func<TItem, Task<bool>> OnSaving { get; set; }
    public Action<TItem> OnSaved { get; set; }
    public Dictionary<string, List<IBrowserFile>> Files { get; } = [];

    public string ClassName
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(Class))
                return Class;

            return SmallLabel ? "kui-small" : "kui-form";
        }
    }

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

        var title = Language?.GetString(Table.Module);
        if (Table.FormTitle != null)
            title = Table.FormTitle.Invoke(Data);
        return Language?.GetFormTitle(Action, title);
    }

    public bool Validate()
    {
        if (OnValidate == null)
            return true;

        return OnValidate.Invoke();
    }

    public async Task CloseAsync()
    {
        if (OnClose != null)
            await OnClose.Invoke();
        OnClosed?.Invoke();
    }

    public Task SaveAsync(Action<TItem> onSaved, bool isClose = true)
    {
        OnSaved = onSaved;
        return SaveAsync(isClose);
    }

    public async Task SaveAsync(bool isClose = true)
    {
        if (!Validate())
            return;

        if (OnSaving != null)
        {
            if (!await OnSaving.Invoke(Data))
                return;
        }

        var confirmText = ConfirmText;
        if (string.IsNullOrWhiteSpace(confirmText))
            confirmText = OnConfirmText?.Invoke();

        if (string.IsNullOrWhiteSpace(confirmText))
        {
            await OnSaveAsync(isClose);
            return;
        }

        UI.Confirm(confirmText, async () => await OnSaveAsync(isClose));
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

    internal void HandleResult(Result result, bool isClose = true)
    {
        UI.Result(result, async () =>
        {
            Data = result.DataAs<TItem>();
            OnSaved?.Invoke(Data);
            if (isClose && result.IsClose)
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

        Width = info.Width;
        Maximizable = info.Maximizable;
        DefaultMaximized = info.DefaultMaximized;
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
            var allColumns = TypeHelper.Properties(typeof(TItem)).Select(p => new ColumnInfo(p)).ToList();
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

enum FormViewType { View, Submit, Verify }