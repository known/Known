namespace Known.Extensions;

static class DictionaryExtension
{
    internal static async Task<List<CodeInfo>> GetDictionariesAsync(this Database db)
    {
        var entities = await db.QueryListAsync<SysDictionary>();
        var codes = entities.Where(d => d.Enabled).OrderBy(d => d.Category).ThenBy(d => d.Sort).Select(e =>
        {
            var code = e.Code;
            var name = string.IsNullOrWhiteSpace(e.Name)
                     ? e.Code
                     : $"{e.Code}-{e.Name}";
            return new CodeInfo(e.Category, code, name, e);
        }).ToList();
        return codes;
    }
}