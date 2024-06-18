namespace Known.Core.Controllers;

[ApiController]
public class AutoController : BaseController, IAutoService
{
    private readonly IAutoService service;

    public AutoController(IAutoService service)
    {
        service.Context = Context;
        this.service = service;
    }

    [HttpPost("QueryModels")]
    public Task<PagingResult<Dictionary<string, object>>> QueryModelsAsync(PagingCriteria criteria) => service.QueryModelsAsync(criteria);

    [HttpPost("DeleteModels")]
    public Task<Result> DeleteModelsAsync(AutoInfo<List<Dictionary<string, object>>> info) => service.DeleteModelsAsync(info);

    [HttpPost("SaveModel")]
    public Task<Result> SaveModelAsync(UploadInfo<Dictionary<string, object>> info) => service.SaveModelAsync(info);
}