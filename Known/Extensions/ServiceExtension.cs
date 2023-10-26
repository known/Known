namespace Known.Extensions;

public static class ServiceExtension
{
    public static async Task RefreshCache(this DictionaryService service)
    {
        var result = await service.RefreshCacheAsync();
        var codes = result.DataAs<List<CodeInfo>>();
        Cache.AttachCodes(codes);
    }
}