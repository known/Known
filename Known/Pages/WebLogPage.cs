namespace Known.Pages;

/// <summary>
/// 系统跟踪日志开发插件页面组件类。
/// </summary>
[Route("/dev/weblog")]
[DevPlugin("跟踪日志", "exception", Sort = 97)]
public class WebLogPage : BaseTablePage<LogInfo>
{
    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        if (!CurrentUser.IsSystemAdmin())
        {
            Navigation.GoErrorPage("403");
            return;
        }

        await base.OnInitPageAsync();

        Table.Name = PageName;
        Table.EnableEdit = false;
        Table.EnableSort = false;
        Table.ShowPager = true;
        Table.OnQuery = Admin.QueryWebLogsAsync;
        Table.Tips = $"该日志为内存日志，默认保留{Config.App.WebLogDays}天。";

        Table.Clear();
        Table.AddColumn(c => c.Type, true).Width(100).Category(nameof(LogLevel)).Tag();
        Table.AddColumn(c => c.Target, true).Width(100).Filter(false).Category(nameof(LogTarget)).Tag();
        Table.AddColumn(c => c.CreateBy, true).Width(100);
        Table.AddColumn(c => c.CreateTime).Width(140).Filter(false).Type(FieldType.DateTime);
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
        form.IsView = true;
        form.AddRow().AddColumn("信息", b =>
        {
            b.Tags(row.Type, row.Target, row.CreateBy, row.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss"));
        });
        form.AddRow().AddColumn("内容", b =>
        {
            b.Div("kui-code", () =>
            {
                b.Div("action", () => b.Icon("copy", "复制错误", this.Callback<MouseEventArgs>(e =>
                {
                    JSRuntime.CopyTextAsync(row.Content);
                    UI.Success("复制成功！");
                })));
                b.Pre().Class("error").Child(row.Content);
            });
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