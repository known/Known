namespace Known.Pages.Develop;

/// <summary>
/// 字段管理开发插件页面组件类。
/// </summary>
[Route("/dev/fields")]
[DevPlugin("字段管理", "field-number", Sort = 4)]
public class FieldPage : BaseTablePage<FieldDataInfo>
{
    private IFieldService Service;

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        if (!CurrentUser.IsSystemAdmin())
        {
            Navigation.GoErrorPage("403");
            return;
        }

        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<IFieldService>();

        Table = new TableModel<FieldDataInfo>(this, TableColumnMode.Attribute);
        Table.Name = PageName;
        Table.EnableEdit = false;
        Table.ShowPager = true;
        Table.SelectType = TableSelectType.Checkbox;
        Table.OnQuery = Service.QueryFieldsAsync;

        Table.Column(c => c.Code).Width(150).ViewLink();
        Table.Column(c => c.Name).Width(120).Query();
        Table.Column(c => c.Type).Width(100);
        Table.Column(c => c.Length);

        Table.Toolbar.AddAction(nameof(New));
        Table.Toolbar.AddAction(nameof(DeleteM));
        Table.Toolbar.AddAction(nameof(Fetch), Language.TipFetchField);
        Table.AddAction(nameof(Edit));
        Table.AddAction(nameof(Delete));
    }

    /// <summary>
    /// 弹出新增表单对话框。
    /// </summary>
    public void New() => Table.NewForm(Service.SaveFieldAsync, new FieldDataInfo());

    /// <summary>
    /// 弹出编辑表单对话框。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Edit(FieldDataInfo row) => Table.EditForm(Service.SaveFieldAsync, row);

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Delete(FieldDataInfo row) => Table.Delete(Service.DeleteFieldsAsync, row);

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    public void DeleteM() => Table.DeleteM(Service.DeleteFieldsAsync);

    /// <summary>
    /// 从实体类中提取公用字段信息。
    /// </summary>
    public void Fetch()
    {
        UI.Confirm(Language.ConfirmFetchField, async () =>
        {
            var result = await Service.FetchFieldsAsync();
            UI.Result(result, RefreshAsync);
        });
    }
}