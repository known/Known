namespace Known.Core.Controllers;

[ApiController]
public class SystemController : BaseController, ISystemService
{
    private readonly ISystemService service;

    public SystemController(ISystemService service)
    {
        service.Context = Context;
        this.service = service;
    }

    [HttpPost("QueryTasks")]
    public Task<PagingResult<SysTask>> QueryTasksAsync(PagingCriteria criteria) => service.QueryTasksAsync(criteria);

    [HttpPost("QueryLogs")]
    public Task<PagingResult<SysLog>> QueryLogsAsync(PagingCriteria criteria) => service.QueryLogsAsync(criteria);

    [HttpGet("GetInstall")]
    public Task<InstallInfo> GetInstallAsync() => service.GetInstallAsync();

    [HttpGet("GetSystem")]
    public Task<SystemInfo> GetSystemAsync() => service.GetSystemAsync();

    [HttpGet("GetModule")]
    public Task<SysModule> GetModuleAsync(string id) => service.GetModuleAsync(id);

    [HttpPost("SaveInstall")]
    public Task<Result> SaveInstallAsync(InstallInfo info) => service.SaveInstallAsync(info);

    [HttpPost("SaveSystem")]
    public Task<Result> SaveSystemAsync(SystemInfo info) => service.SaveSystemAsync(info);

    [HttpPost("SaveKey")]
    public Task<Result> SaveKeyAsync(SystemInfo info) => service.SaveKeyAsync(info);

    [HttpPost("AddLog")]
    public Task<Result> AddLogAsync(SysLog log) => service.AddLogAsync(log);
}