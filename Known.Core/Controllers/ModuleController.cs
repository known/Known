namespace Known.Core.Controllers;

[Route("[controller]")]
public class ModuleController : BaseController
{
    private ModuleService Service => new(Context);

    [HttpGet("[action]")]
    public List<SysModule> GetModules() => Service.GetModules();

    [HttpPost("[action]")]
    public Result DeleteModules([FromBody] List<SysModule> models) => Service.DeleteModules(models);

    [HttpPost("[action]")]
    public Result CopyModules([FromBody] List<SysModule> models) => Service.CopyModules(models);

    [HttpPost("[action]")]
    public Result MoveModules([FromBody] List<SysModule> models) => Service.MoveModules(models);

    [HttpPost("[action]")]
    public Result MoveModule([FromBody] SysModule model) => Service.MoveModule(model);

    [HttpPost("[action]")]
    public Result SaveModule([FromBody] object model) => Service.SaveModule(BaseController.GetDynamicModel(model));
}