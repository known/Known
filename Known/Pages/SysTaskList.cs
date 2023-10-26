namespace Known.Pages;

[Route("/tasks")]
public class SysTaskList : KDataGrid<SysTask>
{
    public SysTaskList()
    {
        OrderBy = $"{nameof(SysTask.CreateTime)} desc";
    }

    protected override Task InitPageAsync()
    {
        Column(c => c.Status).Template(BuildTaskStatus).Select(new SelectOption(typeof(TaskStatus).Name));
        return base.InitPageAsync();
    }

    protected override Task<PagingResult<SysTask>> OnQueryDataAsync(PagingCriteria criteria)
    {
        return Platform.System.QueryTasksAsync(criteria);
    }

    private void BuildTaskStatus(RenderTreeBuilder builder, SysTask row)
    {
        var style = StyleType.Default;
        switch (row.Status)
        {
            case TaskStatus.Pending:
                style = StyleType.Info;
                break;
            case TaskStatus.Running:
                style = StyleType.Primary;
                break;
            case TaskStatus.Success:
                style = StyleType.Success;
                break;
            case TaskStatus.Failed:
                style = StyleType.Danger;
                break;
        }
        builder.Tag(style, row.Status);
    }
}