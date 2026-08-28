namespace Known.Blazor;

/// <summary>
/// 页面表格组件类。
/// </summary>
public class BaseTablePage : BaseComponent
{
    /// <summary>
    /// 取得当前页面菜单信息。
    /// </summary>
    public MenuInfo Menu => Context.Current;

    /// <summary>
    /// 异步保存表格模型配置信息。
    /// </summary>
    /// <param name="info">表格模型配置信息。</param>
    /// <returns></returns>
    public virtual Task<Result> SaveSettingAsync(AutoPageInfo info)
    {
        if (Menu.Plugins == null)
            Menu.Plugins = [];
        Menu.Plugins.AddPlugin(info);
        return Admin.SaveMenuAsync(Menu);
    }
}

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
    public TableModel<TItem> Table { get; set; }

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
        if (string.IsNullOrWhiteSpace(Table.Name))
            Table.Name = PageName;
        Table.DefaultQuery = DefaultQuery;
        // 浏览器刷新直达页面时，菜单在页面初始化之后才加载完成，
        // 导致表格列/按钮等配置初始化为空，这里菜单加载完成后重新初始化并刷新
        Context.OnMenusLoaded += OnMenusLoadedAsync;
    }

    /// <inheritdoc />
    protected override Task OnDisposeAsync()
    {
        if (Context != null)
            Context.OnMenusLoaded -= OnMenusLoadedAsync;
        return base.OnDisposeAsync();
    }

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        if (Table == null)
            return;

        builder.TablePage(Table, page => Table.PageComponent = page);
    }

    private void OnMenusLoadedAsync()
    {
        _ = InvokeAsync(async () =>
        {
            try
            {
                if (Table == null || Table.Columns.Count > 0)
                    return;

                Table.Initialize(true);
                StateHasChanged();
                await Table.RefreshAsync();
            }
            catch (Exception ex)
            {
                await OnErrorAsync(ex);
            }
        });
    }
}