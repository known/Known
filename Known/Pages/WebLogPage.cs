namespace Known.Pages;

/// <summary>
/// 系统跟踪日志开发插件页面组件类。
/// </summary>
[StreamRendering]
[Route("/dev/weblog")]
[DevPlugin("跟踪日志", "exception", Sort = 98)]
public class WebLogPage : BaseTablePage<LogInfo>
{
    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        Table.Name = PageName;
        Table.EnableEdit = false;
        Table.ShowPager = true;
        Table.OnQuery = Admin.QueryWebLogsAsync;

        Table.Clear();
        Table.AddColumn(c => c.Type, true).Width(100).Category(nameof(LogLevel)).Template((b, r) => b.Tag(r.Type));
        Table.AddColumn(c => c.Target, true).Width(100).Category(nameof(LogTarget)).Template((b, r) => b.Tag(r.Target));
        Table.AddColumn(c => c.CreateBy, true).Width(100);
        Table.AddColumn(c => c.CreateTime).Width(140).Type(FieldType.DateTime);
        Table.AddColumn(c => c.Content, true);

        Table.Toolbar.AddAction(nameof(DeleteM));
        Table.Toolbar.AddAction(nameof(Clear));
        Table.Toolbar.AddAction(nameof(Export));

        Table.AddAction(nameof(View));
        Table.AddAction(nameof(Delete));
    }

    /// <summary>
    /// 查看数据。
    /// </summary>
    /// <param name="row"></param>
    [Action]
    public void View(LogInfo row)
    {
        var form = new FormModel<LogInfo>(this);
        form.Title = "查看详情";
        form.Class = "kui-form-weblog";
        form.Info = new FormInfo { Width = 800 };
        form.Data = row;
        form.AddRow().AddColumn("信息", b =>
        {
            b.Tag(row.Type);
            b.Tag(row.Target);
            b.Tag(row.CreateBy);
            b.Tag(row.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss"));
        });
        //form.AddRow().AddColumn(c => c.CreateBy, c => c.ReadOnly = true)
        //             .AddColumn(c => c.CreateTime, c => c.ReadOnly = true);
        form.AddRow().AddColumn(c => c.Content, c =>
        {
            c.ReadOnly = true;
            c.Type = FieldType.TextArea;
        });
        UI.ShowForm(form);
    }

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    [Action] public void Delete(LogInfo row) => Table.Delete(Admin.DeleteWebLogsAsync, row);

    /// <summary>
    /// 批量删除数据。
    /// </summary>
    [Action] public void DeleteM() => Table.DeleteM(Admin.DeleteWebLogsAsync);

    /// <summary>
    /// 清空数据。
    /// </summary>
    /// <returns></returns>
    [Action]
    public void Clear()
    {
        UI.Confirm("确定要清空所有日志？", async () =>
        {
            await Admin.ClearWebLogsAsync();
            await Table.RefreshAsync();
        });
    }

    /// <summary>
    /// 导出表格数据。
    /// </summary>
    [Action] public Task Export() => Table.ExportDataAsync();
}