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

    internal override void ViewForm(FormViewType type, TItem row) => Table.ViewForm(type, row);

    /// <summary>
    /// 异步初始化表格页面组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Table = new TableModel<TItem>(this);
        Table.DefaultQuery = DefaultQuery;
        Table.Initialize(this);
    }

    /// <summary>
    /// 构建表格页面组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildPage(RenderTreeBuilder builder) => builder.Table(Table);

    /// <summary>
    /// 异步弹窗显示导入表单。
    /// </summary>
    /// <param name="param">与后端对应的导入参数。</param>
    /// <returns></returns>
    protected Task ShowImportAsync(string param = null) => Table.ShowImportsync(param);

    /// <summary>
    /// 根据导出模式异步导出表格数据，默认按查询结果导出。
    /// </summary>
    /// <param name="mode">导出模式（单页，查询结果，全部）。</param>
    /// <returns></returns>
    protected Task ExportDataAsync(ExportMode mode = ExportMode.Query) => Table.ExportDataAsync(mode);

    /// <summary>
    /// 根据导出模式异步导出表格数据，默认按查询结果导出。
    /// </summary>
    /// <param name="name">导出文件名称。</param>
    /// <param name="mode">导出模式（单页，查询结果，全部）。</param>
    /// <returns></returns>
    protected Task ExportDataAsync(string name, ExportMode mode = ExportMode.Query) => Table.ExportDataAsync(name, mode);
}