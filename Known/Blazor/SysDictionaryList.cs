using Known.Entities;

namespace Known.Blazor;

class SysDictionaryList : BaseTablePage<SysDictionary>
{
    private string category;
    private int total;

	protected override async Task OnInitPageAsync()
	{
		await base.OnInitPageAsync();
		Table.FormTitle = row => $"{Name} - {row.CategoryName}";
		Table.OnQuery = QueryDictionarysAsync;
		Table.RowKey = r => r.Id;
	}

    private async Task<PagingResult<SysDictionary>> QueryDictionarysAsync(PagingCriteria criteria)
    {
        category = criteria.GetQueryValue(nameof(SysDictionary.Category));
        if (string.IsNullOrWhiteSpace(category))
        {
            category = Cache.GetCodes(Constants.DicCategory)?.FirstOrDefault()?.Code;
            criteria.SetQuery(nameof(SysDictionary.Category), QueryType.Equal, category);
        }
        var result = await Platform.Dictionary.QueryDictionarysAsync(criteria);
        total = result.TotalCount;
        return result;
    }

    [Action] public void New() => Table.NewForm(Platform.Dictionary.SaveDictionaryAsync, new SysDictionary { Category = category, CategoryName = category, Sort = total + 1 });
    [Action] public void Edit(SysDictionary row) => Table.EditForm(Platform.Dictionary.SaveDictionaryAsync, row);
    [Action] public void Delete(SysDictionary row) => Table.Delete(Platform.Dictionary.DeleteDictionarysAsync, row);
    [Action] public void DeleteM() => Table.DeleteM(Platform.Dictionary.DeleteDictionarysAsync);
    [Action] public void Import() => ShowImportForm();
}