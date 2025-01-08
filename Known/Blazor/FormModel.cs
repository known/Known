namespace Known.Blazor;

/// <summary>
/// 表单模型信息类。
/// </summary>
/// <typeparam name="TItem">表单数据类型。</typeparam>
public partial class FormModel<TItem> : BaseModel where TItem : class, new()
{
    private List<ColumnInfo> columns = [];

    /// <summary>
    /// 构造函数，创建一个表单模型信息类的实例。
    /// </summary>
    /// <param name="page">表单关联的页面组件。</param>
    /// <param name="isAuto">是否根据表单数据类型自动生成布局，默认否。</param>
    public FormModel(BaseComponent page, bool isAuto = false) : base(page)
    {
        IsDictionary = typeof(TItem) == typeof(Dictionary<string, object>);
        Page = page;
        if (isAuto)
            columns = TypeHelper.Properties(typeof(TItem)).Select(p => new ColumnInfo(p)).Where(c => c.IsForm).ToList();
    }

    internal FormModel(TableModel<TItem> table, bool isAuto = false) : this(table.Page, isAuto)
    {
        Table = table;
        Type = table.FormType ?? Config.FormTypes.GetValueOrDefault($"{typeof(TItem).Name}Form");
        var plugin = table.Context?.Current?.GetTablePageParameter();
        SetFormInfo(plugin?.Form);
    }

    internal bool IsDictionary { get; }
    internal TableModel<TItem> Table { get; }

    /// <summary>
    /// 取得表单关联的页面组件。
    /// </summary>
    public BaseComponent Page { get; }

    /// <summary>
    /// 取得或设置表单标题。
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 取得或设置表单CSS类名。
    /// </summary>
    public string Class { get; set; }

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
    /// 取得表单字段布局行列表。
    /// </summary>
    public List<FormRow<TItem>> Rows { get; } = [];

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
    /// 取得表单附件字段附件数据信息字典。
    /// </summary>
    public Dictionary<string, List<FileDataInfo>> Files { get; } = [];

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

    /// <summary>
    /// 取得或设置表单查看类型。
    /// </summary>
    public FormViewType FormType { get; set; }

    /// <summary>
    /// 获取表单标题。
    /// </summary>
    /// <returns></returns>
    public string GetFormTitle()
    {
        if (!string.IsNullOrWhiteSpace(Title))
            return Title;

        var title = Language?.GetString(Context.Current);
        if (Table?.FormTitle != null)
            title = Table.FormTitle.Invoke(Data);
        return Language?.GetFormTitle(Action, title);
    }

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
}