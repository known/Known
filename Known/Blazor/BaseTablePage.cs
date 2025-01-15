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

    /// <inheritdoc />
    public override Task RefreshAsync() => Table.RefreshAsync();

    /// <inheritdoc />
    public override void ViewForm(FormViewType type, TItem row) => Table.ViewForm(type, row);

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Table = new TableModel<TItem>(this);
        Table.Name = PageName;
        Table.DefaultQuery = DefaultQuery;
        Table.Columns = Table.GetUserColumns();
    }

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder) => builder.Table(Table);
}