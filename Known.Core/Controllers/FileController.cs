namespace Known.Core.Controllers;

[Route("[controller]")]
public class FileController : BaseController
{
    private FileService Service => new(Context);

    [HttpPost("[action]")]
    public PagingResult<SysFile> QueryFiles([FromBody] PagingCriteria criteria) => Service.QueryFiles(criteria);
    
    [HttpGet("[action]")]
    public ImportFormInfo GetImport([FromQuery] string bizId) => Service.GetImport(bizId);

    [HttpGet("[action]")]
    public byte[] GetImportRule([FromQuery] string bizId) => Service.GetImportRule(bizId);

    [HttpGet("[action]")]
    public List<SysFile> GetFiles([FromQuery] string bizId) => Service.GetFiles(bizId);

    [HttpGet("[action]")]
    public FileUrlInfo GetFileUrl([FromQuery] string bizId) => Service.GetFileUrl(bizId);

    [HttpPost("[action]")]
    public Result DeleteFile([FromBody] SysFile file) => Service.DeleteFile(file);

    [HttpPost("[action]")]
    public Result UploadFile([FromForm] string model, [FromForm] IEnumerable<IFormFile> fileFile)
    {
        var info = GetUploadFormInfo(model);
        var files = GetAttachFiles(fileFile);
        info.Files["File"] = files.Count > 0 ? files[0] : null;
        return Service.UploadFile(info);
    }

    [HttpPost("[action]")]
    public Result UploadFiles([FromForm] string model, [FromForm] IEnumerable<IFormFile> fileFiles)
    {
        var info = GetUploadFormInfo(model);
        info.MultiFiles["Files"] = GetAttachFiles(fileFiles);
        return Service.UploadFiles(info);
    }
}