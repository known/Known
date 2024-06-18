namespace Known.Core.Controllers;

[ApiController]
public class FileController : BaseController, IFileService
{
    private readonly IFileService service;

    public FileController(IFileService service)
    {
        service.Context = Context;
        this.service = service;
    }

    [HttpPost("QueryFiles")]
    public Task<PagingResult<SysFile>> QueryFilesAsync(PagingCriteria criteria) => service.QueryFilesAsync(criteria);

    [HttpGet("GetFiles")]
    public Task<List<SysFile>> GetFilesAsync(string bizId) => service.GetFilesAsync(bizId);

    [HttpGet("GetImport")]
    public Task<ImportFormInfo> GetImportAsync(string bizId) => service.GetImportAsync(bizId);

    [HttpGet("GetImportRule")]
    public Task<byte[]> GetImportRuleAsync(string bizId) => service.GetImportRuleAsync(bizId);

    [HttpPost("DeleteFile")]
    public Task<Result> DeleteFileAsync(SysFile file) => service.DeleteFileAsync(file);

    [HttpPost("UploadFiles")]
    public Task<Result> UploadFilesAsync<TModel>(UploadInfo<TModel> info) => service.UploadFilesAsync(info);
}