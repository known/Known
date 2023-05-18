namespace Known.Core.Controllers;

[Route("[controller]")]
public class SettingController : BaseController
{
    private SettingService Service => new(Context);

    [HttpGet("[action]")]
    public List<SysSetting> GetSettings([FromQuery] string bizType) => Service.GetSettings(bizType);

    [HttpGet("[action]")]
    public SysSetting GetSettingByComp([FromQuery] string bizType) => Service.GetSettingByComp(bizType);

    [HttpGet("[action]")]
    public SysSetting GetSettingByUser([FromQuery] string bizType) => Service.GetSettingByUser(bizType);

    [HttpPost("[action]")]
    public Result DeleteSettings([FromBody] List<SysSetting> models) => Service.DeleteSettings(models);

    [HttpPost("[action]")]
    public Result SaveSetting([FromBody] object model) => Service.SaveSetting(GetDynamicModel(model));
}