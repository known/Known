namespace Known.Blazor;

public class FormModel<TItem> : BaseModel where TItem : class, new()
{
    private bool isInitColumns = false;
    private List<ColumnInfo> columns = [];

    public FormModel(BaseComponent page, bool isAuto = false) : base(page.Context)
    {
        IsDictionary = typeof(TItem) == typeof(Dictionary<string, object>);
        Page = page;
        if (isAuto)
        {
            columns = TypeHelper.Properties(typeof(TItem))
                                .Select(p => new ColumnInfo(p))
                                .Where(c => c.IsForm)
                                .ToList();
        }
    }

    internal FormModel(TableModel<TItem> table) : this(table.Page)
    {
        Table = table;
        Type = table.FormType ?? Config.FormTypes.GetValueOrDefault($"{typeof(TItem).Name}Form");
        SetFormInfo(table.Context.Current.Form);
    }

    internal bool IsDictionary { get; }
    internal BaseComponent Page { get; }
    internal TableModel<TItem> Table { get; }
    internal string Action { get; set; }
    public string Title { get; set; }
    public string Class { get; set; }
    public string ConfirmText { get; set; }
    public Func<string> OnConfirmText { get; set; }
    public FormInfo Info { get; set; }
    public bool SmallLabel { get; set; }
    public bool Draggable { get; set; } = true;
    public bool Resizable { get; set; }
    public bool IsView { get; set; }
    public bool IsNew => Action == "New";
    public List<FormRow<TItem>> Rows { get; } = [];
    public Dictionary<string, List<CodeInfo>> Codes { get; } = [];
    public Dictionary<string, FieldModel<TItem>> Fields { get; } = [];
    public Type Type { get; set; }
    public RenderFragment Header { get; set; }
    public RenderFragment Footer { get; set; }
    public Func<bool> OnValidate { get; set; }
    public Func<Task> OnClose { get; set; }
    public Action OnClosed { get; set; }
    public Action<string> OnFieldChanged { get; set; }
    public Func<TItem, Task> OnLoadData { get; set; }
    public Func<UploadInfo<TItem>, Task<Result>> OnSaveFile { get; set; }
    public Func<TItem, Task<Result>> OnSave { get; set; }
    public Func<TItem, Task<bool>> OnSaving { get; set; }
    public Action<TItem> OnSaved { get; set; }
    public Dictionary<string, List<FileDataInfo>> Files { get; } = [];

    public TItem Data { get; set; }
    internal TItem DefaultData { get; set; }
    internal Func<Task<TItem>> DefaultDataAction { get; set; }

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
        if (column == null || string.IsNullOrWhiteSpace(column.Category))
            return null;

        if (Codes == null || Codes.Count == 0)
            return null;

        if (Codes.TryGetValue(column.Category, out List<CodeInfo> value))
            return value;

        return null;
    }

    internal void LoadDefaultData()
    {
        if (!IsNew)
            return;

        var data = GetDefaultData();
        data ??= new TItem();
        Data = data;
    }

    public async Task LoadDefaultDataAsync()
    {
        if (!IsNew)
            return;

        var data = GetDefaultData();
        data ??= await DefaultDataAction?.Invoke();
        data ??= new TItem();
        Data = data;
    }

    public Task LoadDataAsync(TItem data) => OnLoadData?.Invoke(data);

    public bool HasFile(string key)
    {
        if (Files == null)
            return false;

        if (!Files.TryGetValue(key, out List<FileDataInfo> value))
            return false;

        return value.Count > 0;
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

        var title = Language?.GetString(Context.Current);
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

    public Task SaveAsync(bool isClose = true) => OnSaveAsync(isClose, false);
    public Task SaveContinueAsync() => OnSaveAsync(false, true);

    public async Task OnSaveAsync(bool isClose, bool isContinue)
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
            await OnSaveDataAsync(isClose, isContinue);
            return;
        }

        UI.Confirm(confirmText, async () => await OnSaveDataAsync(isClose, isContinue));
    }

    private async Task OnSaveDataAsync(bool isClose, bool isContinue)
    {
        try
        {
            var result = Result.Error("No save action.");
            if (OnSaveFile != null)
            {
                var info = new UploadInfo<TItem>(Data);
                foreach (var file in Files)
                {
                    info.Files[file.Key] = file.Value;
                }
                result = await OnSaveFile.Invoke(info);
            }
            else if (OnSave != null)
            {
                result = await OnSave.Invoke(Data);
            }
            HandleResult(result, isClose, isContinue);
        }
        catch (Exception ex)
        {
            UI.Error(ex.Message);
            Logger.Exception(ex);
        }
    }

    internal void HandleResult(Result result, bool isClose = true, bool isContinue = false)
    {
        UI.Result(result, async () =>
        {
            var data = result.DataAs<TItem>();
            OnSaved?.Invoke(data);
            if (isClose && result.IsClose)
                await CloseAsync();
            if (Table != null)
                await Table.PageRefreshAsync();
            else if (Page != null)
                await Page.RefreshAsync();
            if (isContinue)
                await LoadDataAsync(null);
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

        Info = info;
        columns = GetFormColumns(info);

        if (info.IsContinue)
        {
            Footer = b =>
            {
                if (IsNew)
                    b.Button(Language.SaveContinue, Page.Callback<MouseEventArgs>(async e => await SaveContinueAsync()), "primary");
                b.Button(Language.SaveClose, Page.Callback<MouseEventArgs>(async e => await SaveAsync()), "primary");
                b.Button(Language.Close, Page.Callback<MouseEventArgs>(async e => await CloseAsync()));
            };
        }
    }

    private TItem GetDefaultData()
    {
        if (DefaultData == null)
            return DefaultData;

        if (IsDictionary)
        {
            var data = new Dictionary<string, object>();
            var items = DefaultData as Dictionary<string, object>;
            foreach (var item in items)
            {
                data[item.Key] = item.Value;
            }
            return data as TItem;
        }
        else
        {
            var data = Activator.CreateInstance<TItem>();
            var properties = TypeHelper.Properties(typeof(TItem));
            foreach (var property in properties)
            {
                var value = property.GetValue(DefaultData);
                property.SetValue(data, value, null);
            }
            return data;
        }
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

    public FormRow<TItem> AddColumn<TValue>(string label, Expression<Func<TItem, TValue>> selector, Action<ColumnInfo> action = null)
    {
        return AddColumn(selector, c =>
        {
            c.Label = label;
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