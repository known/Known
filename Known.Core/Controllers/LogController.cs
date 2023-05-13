namespace Known.Core.Controllers;

[Route("[controller]")]
public class LogController : BaseController
{
    private LogService Service => new(Context);

    [HttpPost("[action]")]
    public PagingResult<SysLog> QueryLogs([FromBody] PagingCriteria criteria) => Service.QueryLogs(criteria);

    [HttpPost("[action]")]
    public Result AddLog([FromBody] SysLog log) => Service.AddLog(log);
}