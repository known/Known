namespace Sample.Core.Controllers;

[Route("[controller]")]
public class ApplyController : BaseController
{
    private ApplyService Service => new(Context);

    //Apply
    [HttpPost("[action]")]
    public PagingResult<TbApply> QueryApplys([FromBody] PagingCriteria criteria) => Service.QueryApplys(criteria);

    [HttpPost("[action]")]
    public Result DeleteApplys([FromBody] List<TbApply> models) => Service.DeleteApplys(models);

    //附件参数名称规范：file[字段属性名]s，该实体附件字段名为：BizFile
    [HttpPost("[action]")]
    public Result SaveApply([FromForm] string model, [FromForm] IEnumerable<IFormFile> fileBizFiles)
    {
        var info = GetUploadFormInfo(model);
        info.MultiFiles[nameof(TbApply.BizFile)] = GetAttachFiles(fileBizFiles);
        return Service.SaveApply(info);
    }
}