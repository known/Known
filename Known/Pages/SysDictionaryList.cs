namespace Known.Pages;

//[Authorize]
[StreamRendering]
[Route("/sys/dictionaries")]
public class SysDictionaryList : BaseTablePage<SysDictionary>
{
    private IDictionaryService dictionaryService;
    private List<CodeInfo> categories;
    private bool isAddCategory;
    private CodeInfo category;
    private int total;
    private string searchKey;

    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        dictionaryService = await CreateServiceAsync<IDictionaryService>();

        Table.FormTitle = row => $"{PageName} - {row.CategoryName}";
        Table.RowKey = r => r.Id;
        Table.OnQuery = QueryDictionarysAsync;
        Table.Column(c => c.Category).Template((b, r) => b.Text(r.CategoryName));
        await LoadCategoriesAsync();
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Div("kui-row-28", () =>
        {
            builder.Div("kui-card", () => BuildListBox(builder));
            builder.Div(() => base.BuildPage(builder));
        });
    }

    public override async Task RefreshAsync()
    {
        if (isAddCategory)
        {
            await LoadCategoriesAsync();
            StateChanged();
        }

        await base.RefreshAsync();
    }

    private void BuildListBox(RenderTreeBuilder builder)
    {
        builder.Div("kui-dict-search", () =>
        {
            UI.BuildSearch(builder, new InputModel<string>
            {
                Placeholder = "Search",
                Value = searchKey,
                ValueChanged = this.Callback<string>(value =>
                {
                    searchKey = value;
                    StateChanged();
                })
            });
        });
        var items = categories;
        if (!string.IsNullOrWhiteSpace(searchKey))
            items = items.Where(c => c.Code.Contains(searchKey) || c.Name.Contains(searchKey)).ToList();
        builder.Component<KListBox>()
               .Set(c => c.Items, items)
               .Set(c => c.ItemTemplate, ItemTemplate)
               .Set(c => c.OnItemClick, OnCategoryClick)
               .Build();
    }

    private RenderFragment ItemTemplate(CodeInfo info) => b => b.Text($"{info.Name} ({info.Code})");

    private Task OnCategoryClick(CodeInfo info)
    {
        category = info;
        return Table.RefreshAsync();
    }

    private async Task<PagingResult<SysDictionary>> QueryDictionarysAsync(PagingCriteria criteria)
    {
        if (category == null)
            return default;

        criteria.SetQuery(nameof(SysDictionary.Category), QueryType.Equal, category?.Code);
        var result = await dictionaryService.QueryDictionariesAsync(criteria);
        total = result.TotalCount;
        return result;
    }

    public void AddCategory()
    {
        isAddCategory = true;
        var code = Constants.DicCategory;
        var dicCate = new CodeInfo(code, code, code, null);
        NewForm(dicCate, categories.Count, true);
    }

    public void New() => NewForm(category, total, false);
    public void Edit(SysDictionary row) => Table.EditForm(dictionaryService.SaveDictionaryAsync, row);
    public void Delete(SysDictionary row) => Table.Delete(dictionaryService.DeleteDictionariesAsync, row);
    public void DeleteM() => Table.DeleteM(dictionaryService.DeleteDictionariesAsync);
    public void Import() => ShowImportForm();

    private async Task LoadCategoriesAsync()
    {
        categories = await dictionaryService.GetCategoriesAsync();
        category ??= categories?.FirstOrDefault();
    }

    private void NewForm(CodeInfo info, int sort, bool isCategory)
    {
        isAddCategory = isCategory;
        Table.NewForm(dictionaryService.SaveDictionaryAsync, new SysDictionary
        {
            Category = info?.Code,
            CategoryName = info?.Name,
            Sort = sort + 1
        });
    }
}