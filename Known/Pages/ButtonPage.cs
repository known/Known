namespace Known.Pages;

/// <summary>
/// 按钮管理开发插件页面组件类。
/// </summary>
[Route("/dev/buttons")]
[DevPlugin("按钮管理", "border", Sort = 2)]
public class ButtonPage : BaseTablePage<ButtonInfo>
{
    /// <summary>
    /// 异步初始化页面组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        Table = new TableModel<ButtonInfo>(this, TableColumnMode.Property);
        Table.Name = PageName;
        Table.ShowPager = true;
        Table.OnQuery = Platform.QueryButtonsAsync;

        Table.Column(c => c.Id).Width(150);
        Table.Column(c => c.Name).Width(120).Query();
        Table.Column(c => c.Icon).Width(100);
        Table.Column(c => c.Style).Width(100);
        Table.Column(c => c.Position);

        Table.Toolbar.AddAction(nameof(New));
        Table.Toolbar.AddAction(nameof(DeleteM));
        Table.AddAction(nameof(Edit));
        Table.AddAction(nameof(Delete));
    }

    /// <summary>
    /// 弹出新增表单对话框。
    /// </summary>
    public void New() => Table.NewForm(Platform.SaveButtonAsync, new ButtonInfo());

    /// <summary>
    /// 弹出编辑表单对话框。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Edit(ButtonInfo row) => Table.EditForm(Platform.SaveButtonAsync, row);

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Delete(ButtonInfo row) => Table.Delete(Platform.DeleteButtonsAsync, row);

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    public void DeleteM() => Table.DeleteM(Platform.DeleteButtonsAsync);
}