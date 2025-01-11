namespace Known.Blazor;

/// <summary>
/// 表格Web页面组件基类。
/// </summary>
/// <typeparam name="TItem">表格行数据类型。</typeparam>
public class BaseTablePage<TItem> : BasePage<TItem> where TItem : class, new()
{
    private ReloadContainer container = null;
    private MenuInfo Menu { get; set; }

    /// <summary>
    /// 取得或设置是否启用页面编辑，默认启用。
    /// </summary>
    protected bool EnableEdit { get; set; } = true;

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
        Menu = Context.Current;
        Table = new TableModel<TItem>(this);
        Table.Name = PageName;
        Table.DefaultQuery = DefaultQuery;
    }

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        if (UIConfig.EnableEdit && EnableEdit)
        {
            var model = new DropdownModel
            {
                Icon = "menu",
                TriggerType = "Click",
                Overlay = BuildTableOverlay
            };
            builder.Component<PluginPanel>()
                   .Set(c => c.Class, "table")
                   .Set(c => c.Dropdown, model)
                   .Set(c => c.ChildContent, BuildTable)
                   .Build();
        }
        else
        {
            BuildTable(builder);
        }
    }

    private void BuildTableOverlay(RenderTreeBuilder builder)
    {
        var data = Menu.TablePage.Page ?? new PageInfo();
        var form = new FormModel<PageInfo>(this)
        {
            SmallLabel = true,
            Data = data,
            OnFieldChanged = async v =>
            {
                Menu.TablePage.Page = data;
                Menu.Plugins.AddPlugin(Menu.TablePage);
                await Platform.SaveMenuAsync(Menu);
                Table.Initialize();
                container?.Reload();
            }
        };
        form.AddRow().AddColumn(c => c.ShowPager);
        form.AddRow().AddColumn(c => c.PageSize);
        form.AddRow().AddColumn(c => c.ToolSize);
        builder.Overlay(() =>
        {
            builder.FormTitle("表格设置");
            builder.Form(form);
        });
    }

    private void BuildTable(RenderTreeBuilder builder)
    {
        builder.Component<ReloadContainer>()
               .Set(c => c.ChildContent, b => b.Table(Table))
               .Build(v => container = v);
    }
}