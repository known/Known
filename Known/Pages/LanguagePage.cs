namespace Known.Pages;

/// <summary>
/// 多语言管理开发插件页面组件类。
/// </summary>
[Route("/dev/languages")]
[DevPlugin("语言管理", "global", Sort = 1)]
public class LanguagePage : BaseTablePage<LanguageInfo>
{
    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        Table = new TableModel<LanguageInfo>(this, TableColumnMode.Property);
        Table.Name = PageName;
        Table.EnableEdit = false;
        Table.ShowPager = true;
        Table.OnQuery = Platform.QueryLanguagesAsync;

        Table.Column(c => c.Id).Width(120).ViewLink();
        Table.Column(c => c.Name).Width(120).Query();
        Table.Column(c => c.Icon);

        Table.Toolbar.AddAction(nameof(New));
        Table.AddAction(nameof(Edit));
        Table.AddAction(nameof(Delete));
    }

    /// <summary>
    /// 弹出新增表单对话框。
    /// </summary>
    public void New() => Table.NewForm(Platform.SaveLanguageAsync, new LanguageInfo());

    /// <summary>
    /// 弹出编辑表单对话框。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Edit(LanguageInfo row) => Table.EditForm(Platform.SaveLanguageAsync, row);

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Delete(LanguageInfo row) => Table.Delete(Platform.DeleteLanguagesAsync, row);
}