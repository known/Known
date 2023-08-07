namespace Sample.Core.Controllers;

[Route("[controller]")]
public class ApplyController : BaseController
{
    private ApplyService Service => new(Context);

    //Apply
    [HttpPost("[action]")]
    public PagingResult<TbApply> QueryApplys([FromBody] PagingCriteria criteria) => Service.QueryApplys(criteria);

    [HttpGet("[action]")]
    public TbApply GetDefaultApply([FromQuery] ApplyType bizType) => Service.GetDefaultApply(bizType);

    [HttpPost("[action]")]
    public Result DeleteApplys([FromBody] List<TbApply> models) => Service.DeleteApplys(models);

    //附件参数名称规范：file[字段属性名]，该实体附件字段名为：BizFile
    [HttpPost("[action]")]
    public Result SaveApply([FromForm] string model, [FromForm] IEnumerable<IFormFile> fileBizFile)
    {
        var info = new UploadFormInfo(model);
        info.Files[nameof(TbApply.BizFile)] = GetAttachFiles(fileBizFile);
        return Service.SaveApply(info);
    }
}