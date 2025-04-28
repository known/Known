namespace Known.Extensions;

static class DictionaryExtension
{
    internal static async Task<List<CodeInfo>> GetDictionariesAsync(this Database db)
    {
        var entities = await db.QueryListAsync<SysDictionary>();
        var codes = entities.Where(d => d.Enabled).OrderBy(d => d.Category).ThenBy(d => d.Sort).Select(e =>
        {
            var code = e.Code;
            var name = string.IsNullOrWhiteSpace(e.Name) ? e.Code : e.Name;
            return new CodeInfo(e.Category, code, name, e);
        }).ToList();

        foreach (var item in CoreOption.Funcs)
        {
            var items = item.Invoke();
            if (items != null && items.Count > 0)
                codes.AddRange(items);
        }

        foreach (var item in CoreOption.FuncTasks)
        {
            var items = await item.Invoke(db);
            if (items != null && items.Count > 0)
                codes.AddRange(items);
        }

        return codes;
    }
}