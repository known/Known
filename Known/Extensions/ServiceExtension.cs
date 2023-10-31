namespace Known.Extensions;

static class ServiceExtension
{
    internal static async Task RefreshCache(this DictionaryService service)
    {
        var result = await service.RefreshCacheAsync();
        var codes = result.DataAs<List<CodeInfo>>();
        Cache.AttachCodes(codes);
    }

    internal static void ShowImport(this UIService ui, ImportOption option)
    {
        ui.Show<Importer>($"导入{option.Name}", new Size(450, 220), action: attr => attr.Set(c => c.Option, option));
    }
}