namespace Known.Pages;

[Route("/sys/tasks")]
//[Menu(Constants.System, "后台任务", "control", 4)]
[PagePlugin("后台任务", "control", PagePluginType.Module, AdminLanguage.SystemManage, Sort = 7)]
public class SysTaskList : BaseTablePage<TaskInfo>
{
    private ITaskService Service;

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

    [Action] public void Delete(TaskInfo row) => Table.Delete(Service.DeleteTasksAsync, row);
    [Action] public void DeleteM() => Table.DeleteM(Service.DeleteTasksAsync);
    
    [Action(Title = AdminLanguage.TipResetTaskStatus)]
    public void Reset() => Table.SelectRows(Service.ResetTasksAsync);

    [Action] public Task Export() => Table.ExportDataAsync();
}