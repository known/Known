namespace Known.Core.Controllers;

[ApiController]
public class DictionaryController : BaseController, IDictionaryService
{
    private readonly IDictionaryService service;

    public DictionaryController(IDictionaryService service)
    {
        service.Context = Context;
        this.service = service;
    }

    [HttpPost("QueryDictionaries")]
    public Task<PagingResult<SysDictionary>> QueryDictionariesAsync(PagingCriteria criteria) => service.QueryDictionariesAsync(criteria);

    [HttpGet("GetCategories")]
    public Task<List<CodeInfo>> GetCategoriesAsync() => service.GetCategoriesAsync();

    [HttpPost("DeleteDictionaries")]
    public Task<Result> DeleteDictionariesAsync(List<SysDictionary> models) => service.DeleteDictionariesAsync(models);

    [HttpPost("SaveDictionary")]
    public Task<Result> SaveDictionaryAsync(SysDictionary model) => service.SaveDictionaryAsync(model);
}