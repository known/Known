namespace Known.Core.Controllers;

[ApiController]
public class ModuleController : BaseController, IModuleService
{
    private readonly IModuleService service;

    public ModuleController(IModuleService service)
    {
        service.Context = Context;
        this.service = service;
    }

    [HttpGet("GetModules")]
    public Task<List<SysModule>> GetModulesAsync() => service.GetModulesAsync();

    [HttpPost("DeleteModules")]
    public Task<Result> DeleteModulesAsync(List<SysModule> models) => service.DeleteModulesAsync(models);

    [HttpPost("CopyModules")]
    public Task<Result> CopyModulesAsync(List<SysModule> models) => service.CopyModulesAsync(models);

    [HttpPost("MoveModules")]
    public Task<Result> MoveModulesAsync(List<SysModule> models) => service.MoveModulesAsync(models);

    [HttpPost("MoveModule")]
    public Task<Result> MoveModuleAsync(SysModule model) => service.MoveModuleAsync(model);

    [HttpPost("SaveModule")]
    public Task<Result> SaveModuleAsync(SysModule model) => service.SaveModuleAsync(model);
}