using Known.Entities;
using Known.Extensions;

namespace Known.Razor;

class SysDictionaryList : BasePage<SysDictionary>
{
    private string category;
    private int total;

    public SysDictionaryList()
    {
        //OrderBy = $"{nameof(SysDictionary.Sort)} asc";
    }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Table.FormTitle = row => $"{Name} - {row.CategoryName}";
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

    public void New() => Table.NewForm(Platform.Dictionary.SaveDictionaryAsync, new SysDictionary { Category = category, CategoryName = category, Sort = total + 1 });
    public void Edit(SysDictionary row) => Table.EditForm(Platform.Dictionary.SaveDictionaryAsync, row);
    public void Delete(SysDictionary row) => Table.Delete(Platform.Dictionary.DeleteDictionarysAsync, row);
    public void DeleteM() => Table.DeleteM(Platform.Dictionary.DeleteDictionarysAsync);
    public void Import() => ShowImportForm();
}