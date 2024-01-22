using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class SysDictionaryList : BasePage<SysDictionary>
{
    private string category;
    private int total;
    private TableModel<SysDictionary> tableCate;
    private TableModel<SysDictionary> tableList;

    protected override async Task OnInitPageAsync()
	{
		await base.OnInitPageAsync();

        Page.Type = PageType.Column;
        Page.Spans = [10, 14];
        Page.Contents = [BuildCategory, BuildDictionary];

        tableCate = new TableModel<SysDictionary>(this);
        tableCate.OnQuery = QueryDictionarysAsync;

        tableList = new TableModel<SysDictionary>(this);
        tableList.FormTitle = row => $"{PageName} - {row.CategoryName}";
        tableList.RowKey = r => r.Id;
        tableList.OnQuery = QueryDictionarysAsync;
	}

    private void BuildCategory(RenderTreeBuilder builder) => builder.BuildTablePage(tableCate);
    private void BuildDictionary(RenderTreeBuilder builder) => builder.BuildTablePage(tableList);

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

    [Action] public void New() => tableList.NewForm(Platform.Dictionary.SaveDictionaryAsync, new SysDictionary { Category = category, CategoryName = category, Sort = total + 1 });
    [Action] public void Edit(SysDictionary row) => tableList.EditForm(Platform.Dictionary.SaveDictionaryAsync, row);
    [Action] public void Delete(SysDictionary row) => tableList.Delete(Platform.Dictionary.DeleteDictionarysAsync, row);
    [Action] public void DeleteM() => tableList.DeleteM(Platform.Dictionary.DeleteDictionarysAsync);
    //[Action] public void Import() => ShowImportForm();
}