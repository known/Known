namespace Known.Plugins.Tables;

/// <summary>
/// 表格标签表单基类。
/// </summary>
public class BaseTableTabForm : BaseTabForm
{
    internal ICodeService CodeService;

    /// <summary>
    /// 表格模型实例。
    /// </summary>
    protected TableModel<Dictionary<string, object>> Table;

    internal string activeTab = "Basic";
    internal bool isPreviewPage = true;
    internal bool isPreviewForm = true;

    /// <summary>
    /// 页面表格组件实例。
    /// </summary>
    protected PageTableForm pageTable;

    /// <summary>
    /// 表单表格组件实例。
    /// </summary>
    protected FormTableForm formTable;

    internal string PageWrapClass => $"kui-plugin-split{(isPreviewPage ? " preview" : "")}";

    [Inject] internal ICodeGenerator Generator { get; set; }

    /// <summary>
    /// 取得或设置泛型表单组件模型实例。
    /// </summary>
    [Parameter] public FormModel<AutoPageInfo> Model { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        CodeService = await CreateServiceAsync<ICodeService>();

        Table = new DemoTableModel(this);
        PreviewPage();
    }

    /// <summary>
    /// 标签页切换事件。
    /// </summary>
    /// <param name="tab">当前激活的标签页。</param>
    protected virtual void OnTabChange(string tab)
    {
        activeTab = tab;
    }

    /// <summary>
    /// 改变模型。
    /// </summary>
    public virtual void ChangeModel() { }

    /// <summary>
    /// 改变模型。
    /// </summary>
    /// <param name="value">模型对象。</param>
    public virtual void ChangeModel(object value) { }

    /// <summary>
    /// 预览页面。
    /// </summary>
    protected virtual void PreviewPage()
    {
        Table.Initialize(Model.Data);
    }
}