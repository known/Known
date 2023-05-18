namespace Known.Core.Controllers;

[Route("[controller]")]
public class DictionaryController : BaseController
{
    private DictionaryService Service => new(Context);

    [HttpPost("[action]")]
    public Result RefreshCache() => Service.RefreshCache();

    [HttpPost("[action]")]
    public PagingResult<SysDictionary> QueryDictionarys([FromBody] PagingCriteria criteria) => Service.QueryDictionarys(criteria);

    [HttpPost("[action]")]
    public Result DeleteDictionarys([FromBody] List<SysDictionary> models) => Service.DeleteDictionarys(models);

    [HttpPost("[action]")]
    public Result SaveDictionary([FromBody] object model) => Service.SaveDictionary(GetDynamicModel(model));
}