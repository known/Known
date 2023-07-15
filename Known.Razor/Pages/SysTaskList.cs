namespace Known.Razor.Pages;

class SysTaskList : DataGrid<SysTask>
{
    public SysTaskList()
    {
        OrderBy = $"{nameof(SysTask.CreateTime)} desc";
    }

    protected override Task InitPageAsync()
    {
        Column(c => c.Status).Select(new SelectOption(typeof(TaskStatus).Name));
        return base.InitPageAsync();
    }

    protected override Task<PagingResult<SysTask>> OnQueryData(PagingCriteria criteria)
    {
        return Platform.System.QueryTasksAsync(criteria);
    }
}