namespace Known.Pages;

/// <summary>
/// 系统后台任务页面组件类。
/// </summary>
[Route("/sys/tasks")]
//[Menu(Constants.System, "后台任务", "control", 4)]
[PagePlugin("后台任务", "control", PagePluginType.Module, AdminLanguage.SystemManage, Sort = 7)]
public class SysTaskList : BaseTablePage<TaskInfo>
{
    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Table.AdvSearch = UIConfig.IsAdvAdmin;
        Table.EnableFilter = UIConfig.IsAdvAdmin;
        Table.OnQuery = Admin.QueryTasksAsync;
        Table.ActionWidth = "70";
        Table.Column(c => c.Status).Tag();
        Table.Column(c => c.BeginTime).Type(FieldType.DateTime);
        Table.Column(c => c.EndTime).Type(FieldType.DateTime);
    }

    /// <summary>
    /// 设置后台任务。
    /// </summary>
    [Action]
    public void Setting()
    {
        if (UIConfig.OnTaskSetting == null)
        {
            UI.Error(AdminLanguage.TipNoConfigOnTaskSetting);
            return;
        }

        UIConfig.OnTaskSetting.Invoke(UI);
    }

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    [Action] public void Delete(TaskInfo row) => Table.Delete(Admin.DeleteTasksAsync, row);

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    [Action] public void DeleteM() => Table.DeleteM(Admin.DeleteTasksAsync);

    /// <summary>
    /// 批量重置后台任务。
    /// </summary>
    [Action(Title = AdminLanguage.TipResetTaskStatus)]
    public void Reset() => Table.SelectRows(Admin.ResetTasksAsync);

    /// <summary>
    /// 导出表格数据。
    /// </summary>
    [Action] public Task Export() => Table.ExportDataAsync();
}