namespace Known.Blazor;

/// <summary>
/// 表单模型信息类。
/// </summary>
/// <typeparam name="TItem">表单数据类型。</typeparam>
public partial class FormModel<TItem> : BaseModel where TItem : class, new()
{
    /// <summary>
    /// 取得表单字段字典。
    /// </summary>
    public Dictionary<string, ColumnInfo> Columns { get; } = [];

    /// <summary>
    /// 构造函数，创建一个表单模型信息类的实例。
    /// </summary>
    /// <param name="page">表单关联的页面组件。</param>
    /// <param name="isAuto">是否根据表单数据类型自动生成布局，默认否。</param>
    public FormModel(IBaseComponent page, bool isAuto = false) : base(page)
    {
        IsDictionary = typeof(TItem).IsDictionary();
        Page = page;
        if (isAuto)
            Columns = TypeCache.Model<TItem>().GetFormns();
    }

    internal FormModel(TableModel table, bool isAuto = false) : this(table.Page, isAuto)
    {
        Table = table;
        Type = table.FormType ?? Config.FormTypes.GetValueOrDefault($"{typeof(TItem).Name}Form");
        var info = table.Info?.Form;
        if (info == null)
        {
            var plugin = table.Context?.Current?.GetAutoPageParameter();
            info = plugin?.Form;
        }
        SetFormInfo(info);
    }

    internal bool IsDictionary { get; }
    internal TableModel Table { get; }

    /// <summary>
    /// 取得表单关联的页面组件。
    /// </summary>
    public IBaseComponent Page { get; }

    /// <summary>
    /// 取得或设置表单标题。
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 取得或设置表单对话框CSS类名。
    /// </summary>
    public string WrapClass { get; set; }

    /// <summary>
    /// 取得或设置表单CSS类名。
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// 取得或设置表单配置信息。
    /// </summary>
    public FormInfo Info { get; set; }

    /// <summary>
    /// 取得或设置是否启用在线编辑。
    /// </summary>
    public bool EnableEdit { get; set; }

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
    /// 取得表单组件参数字典。
    /// </summary>
    public Dictionary<string, object> Parameters { get; } = [];

    /// <summary>
    /// 取得或设置表单对话框头部自定义组件。
    /// </summary>
    public RenderFragment Header { get; set; }

    /// <summary>
    /// 取得或设置表单对话框底部自定义组件。
    /// </summary>
    public RenderFragment Footer { get; set; }

    /// <summary>
    /// 取得或设置表单对话框底部左侧自定义组件。
    /// </summary>
    public RenderFragment FooterLeft { get; set; }

    /// <summary>
    /// 取得或设置表单对话框底部右侧按钮左侧自定义组件。
    /// </summary>
    public RenderFragment FooterRight { get; set; }

    /// <summary>
    /// 取得表单附件字段附件数据信息字典。
    /// </summary>
    public Dictionary<string, List<FileDataInfo>> Files { get; } = [];

    /// <summary>
    /// 取得表单CSS类名。
    /// </summary>
    public string ClassName => CssBuilder.Default(SmallLabel ? "kui-small" : "kui-form").AddClass(Class).BuildClass();

    /// <summary>
    /// 取得或设置表单查看类型， 默认View-查看。
    /// </summary>
    public FormViewType ViewType { get; set; }

    internal bool IsTabForm => Type?.IsSubclassOf(typeof(BaseTabForm)) == true;
    internal bool IsStepForm => Type?.IsSubclassOf(typeof(BaseStepForm)) == true;
    internal bool IsNoFooter => IsTabForm || IsStepForm || Info?.NoFooter == true;

    /// <summary>
    /// 获取表单标题。
    /// </summary>
    /// <returns></returns>
    public string GetFormTitle()
    {
        if (!string.IsNullOrWhiteSpace(Title))
            return Language?[Title];

        var title = Table?.Name;
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