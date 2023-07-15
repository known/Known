namespace Known.Razor.Pages;

class SysLogList : DataGrid<SysLog>
{
    public SysLogList()
    {
        OrderBy = $"{nameof(SysLog.CreateTime)} desc";
    }

    protected override Task<PagingResult<SysLog>> OnQueryData(PagingCriteria criteria)
    {
        return Platform.System.QueryLogsAsync(criteria);
    }
}