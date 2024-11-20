namespace Known.Blazor;

/// <summary>
/// 表格Web页面组件基类。
/// </summary>
/// <typeparam name="TItem">表格行数据类型。</typeparam>
public class BaseTablePage<TItem> : BasePage<TItem> where TItem : class, new()
{
    /// <summary>
    /// 取得或设置表格页面默认查询条件匿名对象，对象属性名应与查询实体对应。
    /// </summary>
    protected object DefaultQuery { get; set; }

    /// <summary>
    /// 取得或设置页面表格组件模型对象实例。
    /// </summary>
    protected TableModel<TItem> Table { get; set; }

    /// <summary>
    /// 取得页面表格选中行对象列表。
    /// </summary>
    public IEnumerable<TItem> SelectedRows => Table.SelectedRows;

    /// <summary>
    /// 刷新页面表格组件。
    /// </summary>
    /// <returns></returns>
    public override Task RefreshAsync() => Table.RefreshAsync();

    /// <summary>
    /// 查看表单。
    /// </summary>
    /// <param name="type">查看类型。</param>
    /// <param name="row">表单数据。</param>
    public override void ViewForm(FormViewType type, TItem row) => Table.ViewForm(type, row);

    /// <summary>
    /// 异步初始化表格页面组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Table = new TableModel<TItem>(this);
        Table.DefaultQuery = DefaultQuery;
    }

    /// <summary>
    /// 构建表格页面组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildPage(RenderTreeBuilder builder) => builder.Table(Table);
}