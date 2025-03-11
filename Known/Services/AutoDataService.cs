namespace Known.Services;

class AutoDataService(IAutoPage page, IAutoService service)
{
    private readonly IAutoPage Page = page;
    private readonly IAutoService Service = service;
    private readonly AutoService AutoDemo = new(page.Context);

    private bool IsPrototype => Page.Context.Current?.Type == nameof(MenuType.Prototype);

    internal static async Task<AutoDataService> CreateServiceAsync(IAutoPage page)
    {
        var autoService = await page.CreateServiceAsync<IAutoService>();
        return new AutoDataService(page, autoService);
    }

    internal Task<PagingResult<Dictionary<string, object>>> QueryModelsAsync(PagingCriteria criteria)
    {
        criteria.Parameters[nameof(IAutoPage.PageId)] = Page.PageId;
        return IsPrototype ? AutoDemo.QueryModelsAsync(criteria) : Service.QueryModelsAsync(criteria);
    }

    internal Task<Dictionary<string, object>> GetModelAsync(string id)
    {
        return IsPrototype ? AutoDemo.GetModelAsync(Page.PageId, id) : Service.GetModelAsync(Page.PageId, id);
    }

    internal Task<Result> DeleteModelsAsync(List<Dictionary<string, object>> models)
    {
        var info = new AutoInfo<List<Dictionary<string, object>>> { PageId = Page.PageId, Data = models };
        return IsPrototype ? AutoDemo.DeleteModelsAsync(info) : Service.DeleteModelsAsync(info);
    }

    internal Task<Result> SaveModelAsync(UploadInfo<Dictionary<string, object>> info)
    {
        info.PageId = Page.PageId;
        return IsPrototype ? AutoDemo.SaveModelAsync(info) : Service.SaveModelAsync(info);
    }
}