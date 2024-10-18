namespace Known.Pages;

/// <summary>
/// 泛型实体自动表格页面。
/// </summary>
/// <typeparam name="TEntity">实体类型。</typeparam>
public class EntityTablePage<TEntity> : BaseTablePage<TEntity> where TEntity : class, new()
{
    private IEntityService<TEntity> Service;

    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnPageInitAsync()
    {
        Table = new TableModel<TEntity>(this, true);
        Table.DefaultQuery = DefaultQuery;

        Service = await CreateServiceAsync<IEntityService<TEntity>>();
        Table.OnQuery = Service.QueryAsync;
        Table.Toolbar.AddAction(nameof(New));
        Table.Toolbar.AddAction(nameof(DeleteM));

        Table.AddAction(nameof(Edit));
        Table.AddAction(nameof(Delete));
    }

    /// <summary>
    /// 弹出新增表单对话框。
    /// </summary>
    public void New() => Table.NewForm(Service.SaveAsync, new TEntity());

    /// <summary>
    /// 弹出编辑表单对话框。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Edit(TEntity row) => Table.EditForm(Service.SaveAsync, row);

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Delete(TEntity row) => Table.Delete(Service.DeleteAsync, row);

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    public void DeleteM() => Table.DeleteM(Service.DeleteAsync);
}