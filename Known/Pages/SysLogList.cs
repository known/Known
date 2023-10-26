namespace Known.Pages;

[Route("/logs")]
public class SysLogList : KDataGrid<SysLog>
{
    public SysLogList()
    {
        OrderBy = $"{nameof(SysLog.CreateTime)} desc";
    }

    protected override Task<PagingResult<SysLog>> OnQueryDataAsync(PagingCriteria criteria)
    {
        return Platform.System.QueryLogsAsync(criteria);
    }
}