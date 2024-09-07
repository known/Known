namespace Known.Components;

/// <summary>
/// 表格弹出选择器组件类。
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class TablePicker<TItem> : BasePicker<TItem> where TItem : class, new()
{
    /// <summary>
    /// 取得表格组件配置模型对象。
    /// </summary>
    protected TableModel<TItem> Table { get; private set; }

    /// <summary>
    /// 取得表格选中行绑定的数据对象列表。
    /// </summary>
    public override List<TItem> SelectedItems => Table.SelectedRows?.ToList();

    /// <summary>
    /// 异步初始化表格选择器组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        IsMulti = false;
        Table = new TableModel<TItem>(this)
        {
            IsForm = true,
            AdvSearch = false,
            ShowPager = true,
            SelectType = TableSelectType.Radio
        };
    }

    /// <summary>
    /// 呈现表格选择器内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildContent(RenderTreeBuilder builder) => builder.Table(Table);
}

/// <summary>
/// 系统用户弹窗选择器组件类。
/// </summary>
public class UserPicker : TablePicker<SysUser>
{
    private IUserService Service;

    /// <summary>
    /// 异步初始化系统用户弹窗选择器组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<IUserService>();
        Title = Language["Title.SelectUser"];
        Table.OnQuery = Service.QueryUsersAsync;
        Table.AddColumn(c => c.UserName).Width(100);
        Table.AddColumn(c => c.Name, true).Width(100);
        Table.AddColumn(c => c.Phone).Width(100);
        Table.AddColumn(c => c.Email).Width(100);
        Table.AddColumn(c => c.Role);
    }
}