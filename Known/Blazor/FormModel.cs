namespace Known.Blazor;

/// <summary>
/// 表单模型信息类。
/// </summary>
/// <typeparam name="TItem">表单数据类型。</typeparam>
public class FormModel<TItem> : BaseModel where TItem : class, new()
{
    private bool isInitColumns = false;
    private List<ColumnInfo> columns = [];

    /// <summary>
    /// 构造函数，创建一个表单模型信息类的实例。
    /// </summary>
    /// <param name="page">表单关联的页面。</param>
    /// <param name="isAuto">是否根据表单数据类型自动生成布局，默认否。</param>
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

    internal FormModel(TableModel<TItem> table, bool isAuto = false) : this(table.Page, isAuto)
    {
        Table = table;
        Type = table.FormType ?? Config.FormTypes.GetValueOrDefault($"{typeof(TItem).Name}Form");
        SetFormInfo(table.Context.Current.Form);
    }

    internal bool IsDictionary { get; }
    internal BaseComponent Page { get; }
    internal TableModel<TItem> Table { get; }
    internal string Action { get; set; }

    /// <summary>
    /// 取得或设置表单标题。
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 取得或设置表单CSS类名。
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// 取得或设置表单保存确认提示框信息。
    /// </summary>
    public string ConfirmText { get; set; }

    /// <summary>
    /// 取得或设置表单保存确认提示框信息回调方法。
    /// </summary>
    public Func<string> OnConfirmText { get; set; }

    /// <summary>
    /// 取得或设置表单配置信息。
    /// </summary>
    public FormInfo Info { get; set; }

    /// <summary>
    /// 取得或设置表单是否窄宽标题。
    /// </summary>
    public bool SmallLabel { get; set; }

    /// <summary>
    /// 取得或设置表单对话框是否可拖动。
    /// </summary>
    public bool Draggable { get; set; } = true;

    /// <summary>
    /// 取得或设置表单对话框是否可调整大小。
    /// </summary>
    public bool Resizable { get; set; }

    /// <summary>
    /// 取得或设置表单是否是查看模式。
    /// </summary>
    public bool IsView { get; set; }

    /// <summary>
    /// 取得表单是否是新增表单，当Action为New时。
    /// </summary>
    public bool IsNew => Action == "New";

    /// <summary>
    /// 取得表单字段布局行列表。
    /// </summary>
    public List<FormRow<TItem>> Rows { get; } = [];

    /// <summary>
    /// 取得表单字段代码表字典。
    /// </summary>
    public Dictionary<string, List<CodeInfo>> Codes { get; } = [];

    /// <summary>
    /// 取得表单字段模型信息字典。
    /// </summary>
    public Dictionary<string, FieldModel<TItem>> Fields { get; } = [];

    /// <summary>
    /// 取得或设置表单的组件类型。
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// 取得或设置表单对话框头部自定义组件。
    /// </summary>
    public RenderFragment Header { get; set; }

    /// <summary>
    /// 取得或设置表单对话框底部自定义组件。
    /// </summary>
    public RenderFragment Footer { get; set; }

    /// <summary>
    /// 取得或设置表单验证委托，当呈现抽象UI表单赋值。
    /// </summary>
    public Func<bool> OnValidate { get; set; }

    /// <summary>
    /// 取得或设置表单对话框关闭委托，显示对话框时赋值。
    /// </summary>
    public Func<Task> OnClose { get; set; }

    /// <summary>
    /// 取得或设置表单关闭操作时调用的委托。
    /// </summary>
    public Action OnClosed { get; set; }

    /// <summary>
    /// 取得或设置表单字段值改变时调用的委托。
    /// </summary>
    public Action<string> OnFieldChanged { get; set; }

    /// <summary>
    /// 取得或设置表单加载时调用的委托。
    /// </summary>
    public Func<TItem, Task> OnLoadData { get; set; }

    /// <summary>
    /// 取得或设置附件表单保存时调用的委托。
    /// </summary>
    public Func<UploadInfo<TItem>, Task<Result>> OnSaveFile { get; set; }

    /// <summary>
    /// 取得或设置表单保存时调用的委托。
    /// </summary>
    public Func<TItem, Task<Result>> OnSave { get; set; }

    /// <summary>
    /// 取得或设置表单保存前调用的委托。
    /// </summary>
    public Func<TItem, Task<bool>> OnSaving { get; set; }

    /// <summary>
    /// 取得或设置表单保存后调用的委托。
    /// </summary>
    public Action<TItem> OnSaved { get; set; }

    /// <summary>
    /// 取得或设置表单保存后调用的委托。
    /// </summary>
    public Func<TItem, Task> OnSavedAsync { get; set; }

    /// <summary>
    /// 取得表单附件字段附件数据信息字典。
    /// </summary>
    public Dictionary<string, List<FileDataInfo>> Files { get; } = [];

    /// <summary>
    /// 取得或设置表单关联的数据对象。
    /// </summary>
    public TItem Data { get; set; }

    internal TItem DefaultData { get; set; }
    internal Func<Task<TItem>> DefaultDataAction { get; set; }

    /// <summary>
    /// 取得表单CSS类名。
    /// </summary>
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

    /// <summary>
    /// 异步加载表单默认数据。
    /// </summary>
    /// <returns></returns>
    public async Task LoadDefaultDataAsync()
    {
        if (!IsNew)
            return;

        var data = GetDefaultData();
        data ??= await DefaultDataAction?.Invoke();
        data ??= new TItem();
        Data = data;
    }

    /// <summary>
    /// 异步加载表单数据。
    /// </summary>
    /// <param name="data">表单数据。</param>
    /// <returns></returns>
    public Task LoadDataAsync(TItem data) => OnLoadData?.Invoke(data);

    /// <summary>
    /// 判断表单是否有附件。
    /// </summary>
    /// <param name="key">附件字段ID。</param>
    /// <returns>是否有附件。</returns>
    public bool HasFile(string key)
    {
        if (Files == null)
            return false;

        if (!Files.TryGetValue(key, out List<FileDataInfo> value))
            return false;

        return value.Count > 0;
    }

    /// <summary>
    /// 以编码方式添加表单行布局。
    /// </summary>
    /// <param name="action">行内子组件委托。</param>
    /// <returns>表单行对象。</returns>
    public FormRow<TItem> AddRow(Action<FormRow<TItem>> action = null)
    {
        var row = new FormRow<TItem>(this);
        action?.Invoke(row);
        Rows.Add(row);
        return row;
    }

    /// <summary>
    /// 根据属性选择表达式返回表单字段栏位建造者。
    /// </summary>
    /// <typeparam name="TValue">字段属性类型。</typeparam>
    /// <param name="selector">属性选择表达式。</param>
    /// <returns>字段栏位建造者。</returns>
    public ColumnBuilder<TItem> Field<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        var property = TypeHelper.Property(selector);
        var column = columns?.FirstOrDefault(c => c.Id == property.Name);
        return new ColumnBuilder<TItem>(column);
    }

    /// <summary>
    /// 初始化表单布局。
    /// </summary>
    public void Initialize()
    {
        if (isInitColumns)
            return;

        isInitColumns = true;
        InitColumns();
    }

    /// <summary>
    /// 获取表单标题。
    /// </summary>
    /// <returns></returns>
    public string GetFormTitle()
    {
        if (!string.IsNullOrWhiteSpace(Title))
            return Title;

        var title = Language?.GetString(Context.Current);
        if (Table.FormTitle != null)
            title = Table.FormTitle.Invoke(Data);
        return Language?.GetFormTitle(Action, title);
    }

    /// <summary>
    /// 验证表单字段。
    /// </summary>
    /// <returns>是否通过。</returns>
    public bool Validate()
    {
        if (OnValidate == null)
            return true;

        return OnValidate.Invoke();
    }

    /// <summary>
    /// 关闭表单对话框。
    /// </summary>
    /// <returns></returns>
    public async Task CloseAsync()
    {
        if (OnClose != null)
            await OnClose.Invoke();
        OnClosed?.Invoke();
    }

    /// <summary>
    /// 异步保存表单数据。
    /// </summary>
    /// <param name="onSaved">保存后委托。</param>
    /// <param name="isClose">是否关闭对话框，默认是。</param>
    /// <returns></returns>
    public Task SaveAsync(Action<TItem> onSaved, bool isClose = true)
    {
        OnSaved = onSaved;
        return SaveAsync(isClose);
    }

    /// <summary>
    /// 异步保存表单数据。
    /// </summary>
    /// <param name="onSavedAsync ">异步保存后委托。</param>
    /// <param name="isClose">是否关闭对话框，默认是。</param>
    /// <returns></returns>
    public Task SaveAsync(Func<TItem, Task> onSavedAsync, bool isClose = true)
    {
        OnSavedAsync = onSavedAsync;
        return SaveAsync(isClose);
    }

    /// <summary>
    /// 保存表单数据。
    /// </summary>
    /// <param name="isClose">是否关闭对话框，默认是。</param>
    /// <returns></returns>
    public Task SaveAsync(bool isClose = true) => OnSaveAsync(isClose, false);

    /// <summary>
    /// 保存表单数据继续添加新数据。
    /// </summary>
    /// <returns></returns>
    public Task SaveContinueAsync() => OnSaveAsync(false, true);

    /// <summary>
    /// 保存表单数据。
    /// </summary>
    /// <param name="isClose">是否关闭对话框。</param>
    /// <param name="isContinue">是否继续添加新数据。</param>
    /// <returns></returns>
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

    internal void HandleResult(Result result, bool isClose = true, bool isContinue = false)
    {
        UI.Result(result, async () =>
        {
            var data = result.DataAs<TItem>();
            if (OnSaved != null)
                OnSaved?.Invoke(data);
            else if (OnSavedAsync != null)
                await OnSavedAsync.Invoke(data);

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

    /// <summary>
    /// 初始化无代码表单栏位。
    /// </summary>
    public void InitColumns()
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

    /// <summary>
    /// 设置无代码表单信息。
    /// </summary>
    /// <param name="info"></param>
    public void SetFormInfo(FormInfo info)
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
                    b.Button(Language.SaveContinue, Page.Callback<MouseEventArgs>(async e => await SaveContinueAsync()));
                b.Button(Language.SaveClose, Page.Callback<MouseEventArgs>(async e => await SaveAsync()));
                b.Button(Language.Close, Page.Callback<MouseEventArgs>(async e => await CloseAsync()), "default");
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
            var baseProperties = TypeHelper.Properties(typeof(EntityBase));
            var properties = TypeHelper.Properties(typeof(TItem));
            foreach (var item in properties)
            {
                if (!item.CanWrite || baseProperties.Any(p => p.Name == item.Name))
                    continue;

                var value = item.GetValue(DefaultData);
                item.SetValue(data, value, null);
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

/// <summary>
/// 表单行操作类。
/// </summary>
/// <typeparam name="TItem">表单数据类型。</typeparam>
public class FormRow<TItem> where TItem : class, new()
{
    internal FormRow(FormModel<TItem> form)
    {
        Form = form;
    }

    /// <summary>
    /// 取得表单行对应表单模型对象。
    /// </summary>
    public FormModel<TItem> Form { get; }

    /// <summary>
    /// 取得表单行关联的字段列表。
    /// </summary>
    public List<FieldModel<TItem>> Fields { get; } = [];

    /// <summary>
    /// 添加一列表单只读文本字段。
    /// </summary>
    /// <param name="id">字段属性ID。</param>
    /// <param name="text">字段显示文本。</param>
    /// <returns>表单行对象。</returns>
    public FormRow<TItem> AddColumn(string id, string text) => AddColumn(id, b => b.Text(text));

    /// <summary>
    /// 添加一列表单呈现模板字段。
    /// </summary>
    /// <param name="id">字段属性ID。</param>
    /// <param name="template">字段呈现模板。</param>
    /// <returns>表单行对象。</returns>
    public FormRow<TItem> AddColumn(string id, RenderFragment template) => AddColumn(new ColumnInfo(id, template));

    /// <summary>
    /// 添加一列表单字段。
    /// </summary>
    /// <typeparam name="TValue">字段属性类型。</typeparam>
    /// <param name="label">字段标题。</param>
    /// <param name="selector">字段属性选择表达式。</param>
    /// <param name="action">字段参数设置委托方法。</param>
    /// <returns>表单行对象。</returns>
    public FormRow<TItem> AddColumn<TValue>(string label, Expression<Func<TItem, TValue>> selector, Action<ColumnInfo> action = null)
    {
        return AddColumn(selector, c =>
        {
            c.Label = label;
            action?.Invoke(c);
        });
    }

    /// <summary>
    /// 添加一列表单字段。
    /// </summary>
    /// <typeparam name="TValue">字段属性类型。</typeparam>
    /// <param name="selector">字段属性选择表达式。</param>
    /// <param name="action">字段参数设置委托方法。</param>
    /// <returns>表单行对象。</returns>
    public FormRow<TItem> AddColumn<TValue>(Expression<Func<TItem, TValue>> selector, Action<ColumnInfo> action = null)
    {
        var property = TypeHelper.Property(selector);
        var column = new ColumnInfo(property);
        action?.Invoke(column);
        return AddColumn(column);
    }

    /// <summary>
    /// 添加多列表单字段。
    /// </summary>
    /// <param name="columns">表单字段列对象列表。</param>
    /// <returns>表单行对象。</returns>
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