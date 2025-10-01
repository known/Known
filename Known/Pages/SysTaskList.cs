namespace Known.Pages;

/// <summary>
/// 后台任务页面组件类。
/// </summary>
[Route("/sys/tasks")]
[Menu(Constants.System, "后台任务", "control", 4)]
//[PagePlugin("后台任务", "control", PagePluginType.Module, AdminLanguage.SystemManage, Sort = 7)]
public class SysTaskList : BaseTablePage<SysTask>
{
    private ITaskService Service;

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<ITaskService>();

        Table.AdvSearch = UIConfig.IsAdvAdmin;
        Table.EnableFilter = UIConfig.IsAdvAdmin;
        Table.OnQuery = Service.QueryTasksAsync;
        Table.ActionWidth = "70";
        Table.Column(c => c.Status).Tag();
        Table.Column(c => c.BeginTime).Type(FieldType.DateTime);
        Table.Column(c => c.EndTime).Type(FieldType.DateTime);
    }

    /// <summary>
    /// 任务设置。
    /// </summary>
    [Action]
    public void Setting()
    {
        if (UIConfig.OnTaskSetting == null)
        {
            UI.Error(Language.TipNoConfigOnTaskSetting);
            return;
        }

        UIConfig.OnTaskSetting.Invoke(UI);
    }

    /// <summary>
    /// 删除任务。
    /// </summary>
    /// <param name="row">任务信息。</param>
    [Action] public void Delete(SysTask row) => Table.Delete(Service.DeleteTasksAsync, row);

    /// <summary>
    /// 批量删除任务。
    /// </summary>
    [Action] public void DeleteM() => Table.DeleteM(Service.DeleteTasksAsync);
    
    /// <summary>
    /// 重置任务。
    /// </summary>
    [Action(Title = Language.TipResetTaskStatus)]
    public void Reset() => Table.SelectRows(Service.ResetTasksAsync);

    /// <summary>
    /// 导出任务列表。
    /// </summary>
    /// <returns></returns>
    [Action] public Task Export() => Table.ExportDataAsync();
}