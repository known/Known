using Known.Entities;

namespace Known.Blazor;

class SysDictionaryList : BaseTablePage<SysDictionary>
{
    private string category;
    private int total;

	protected override async Task OnInitPageAsync()
	{
		await base.OnInitPageAsync();
		Model.OnQuery = QueryDictionarysAsync;
		Model.RowKey = r => r.Id;
		Model.Column(c => c.Sort).DefaultAscend();
		Model.FormTitle = row => $"{Name} - {row.CategoryName}";
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

    [Action] public void New() => Model.NewForm(Platform.Dictionary.SaveDictionaryAsync, new SysDictionary { Category = category, CategoryName = category, Sort = total + 1 });
    [Action] public void Edit(SysDictionary row) => Model.EditForm(Platform.Dictionary.SaveDictionaryAsync, row);
    [Action] public void Delete(SysDictionary row) => Model.Delete(Platform.Dictionary.DeleteDictionarysAsync, row);
    [Action] public void DeleteM() => Model.DeleteM(Platform.Dictionary.DeleteDictionarysAsync);
    [Action] public void Import() => ShowImportForm();
}