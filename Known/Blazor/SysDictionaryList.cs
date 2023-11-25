using Known.Entities;

namespace Known.Blazor;

class SysDictionaryList : BasePage<SysDictionary>
{
    private string category;
    private int total;

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Page.FormTitle = row => $"{Name} - {row.CategoryName}";
        Page.Table.Column(c => c.Sort).DefaultSort("asc");
    }

    protected override async Task<PagingResult<SysDictionary>> OnQueryAsync(PagingCriteria criteria)
    {
        category = criteria.GetQueryValue(nameof(SysDictionary.Category));
        if (string.IsNullOrWhiteSpace(category))
        {
            category = Cache.GetCodes(Constants.DicCategory, false)?.FirstOrDefault()?.Code;
            criteria.SetQuery(nameof(SysDictionary.Category), QueryType.Equal, category);
        }
        var result = await Platform.Dictionary.QueryDictionarysAsync(criteria);
        total = result.TotalCount;
        return result;
    }

    [Action] public void New() => Page.NewForm(Platform.Dictionary.SaveDictionaryAsync, new SysDictionary { Category = category, CategoryName = category, Sort = total + 1 });
    [Action] public void Edit(SysDictionary row) => Page.EditForm(Platform.Dictionary.SaveDictionaryAsync, row);
    [Action] public void Delete(SysDictionary row) => Page.Delete(Platform.Dictionary.DeleteDictionarysAsync, row);
    [Action] public void DeleteM() => Page.DeleteM(Platform.Dictionary.DeleteDictionarysAsync);
    [Action] public void Import() => ShowImportForm();
}