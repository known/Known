namespace Known.Razor.Extensions;

public static class ClientExtension
{
    public static async Task RefreshCache(this DictionaryClient client)
    {
        var result = await client.RefreshCacheAsync();
        var codes = result.DataAs<List<CodeInfo>>();
        Cache.AttachCodes(codes);
    }
}