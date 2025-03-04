namespace Known.Pages;

/// <summary>
/// 数据字典模块页面组件类。
/// </summary>
[Route("/sys/dictionaries")]
[Menu(Constants.BaseData, "数据字典", "unordered-list", 2)]
public class SysDictionaryList : BaseTablePage<DictionaryInfo>
{
    private List<CodeInfo> categories;
    private CodeInfo category;
    private bool isAddCategory;
    private int total;

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        Table.FormTitle = row => $"{PageName} - {row.CategoryName}";
        Table.RowKey = r => r.Id;
        Table.OnQuery = QueryDictionarysAsync;
        Table.Column(c => c.Category).Template((b, r) => b.Text(r.CategoryName));
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
            await LoadCategoriesAsync();
    }

    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Component<KListTable<DictionaryInfo>>()
               .Set(c => c.ListData, categories)
               .Set(c => c.OnListClick, this.Callback<CodeInfo>(OnItemClickAsync))
               .Set(c => c.Table, Table)
               .Build();
    }

    /// <inheritdoc />
    public override async Task RefreshAsync()
    {
        if (isAddCategory)
            await LoadCategoriesAsync();
        else
            await base.RefreshAsync();
    }

    /// <summary>
    /// 弹出添加类别对话框。
    /// </summary>
    [Action]
    public void AddCategory()
    {
        isAddCategory = true;
        var model = new DialogModel
        {
            Title = Language.GetString("Button.AddCategory"),
            Width = 800,
            Content = b => b.Component<CategoryGrid>()
                            .Set(c => c.OnRefresh, RefreshAsync)
                            .Build()
        };
        UI.ShowDialog(model);
    }

    /// <summary>
    /// 弹出新增表单对话框。
    /// </summary>
    [Action] public void New() => NewForm(category, total);

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    [Action] public void DeleteM() => Table.DeleteM(Admin.DeleteDictionariesAsync);

    /// <summary>
    /// 弹出编辑表单对话框。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    [Action] public void Edit(DictionaryInfo row) => Table.EditForm(Admin.SaveDictionaryAsync, row);

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    [Action] public void Delete(DictionaryInfo row) => Table.Delete(Admin.DeleteDictionariesAsync, row);

    /// <summary>
    /// 弹出数据导入对话框。
    /// </summary>
    [Action] public Task Import() => Table.ShowImportAsync();

    private Task OnItemClickAsync(CodeInfo info)
    {
        category = info;
        return Table.RefreshAsync();
    }

    private async Task<PagingResult<DictionaryInfo>> QueryDictionarysAsync(PagingCriteria criteria)
    {
        if (category == null)
            return default;

        criteria.SetQuery(nameof(DictionaryInfo.Category), QueryType.Equal, category?.Code);
        var result = await Admin.QueryDictionariesAsync(criteria);
        total = result.TotalCount;
        return result;
    }

    private async Task LoadCategoriesAsync()
    {
        categories = await Admin.GetCategoriesAsync();
        await StateChangedAsync();
        category = categories?.FirstOrDefault();
        await OnItemClickAsync(category);
    }

    private void NewForm(CodeInfo info, int sort)
    {
        if (category == null)
        {
            UI.Error(Language["Tip.SelectCategory"]);
            return;
        }

        isAddCategory = false;
        Table.NewForm(Admin.SaveDictionaryAsync, new DictionaryInfo
        {
            Category = info?.Code,
            CategoryName = info?.Name,
            Sort = sort + 1
        });
    }
}

class CategoryGrid : BaseTable<DictionaryInfo>
{
    private readonly CodeInfo category = new(Constants.DicCategory, Constants.DicCategory, Constants.DicCategory, null);
    private int total;

    [Parameter] public Func<Task> OnRefresh { get; set; }

    public override async Task RefreshAsync()
    {
        await base.RefreshAsync();
        await OnRefresh?.Invoke();
    }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Table.AutoHeight = false;
        Table.ShowPager = true;
        Table.OnQuery = QueryDictionariesAsync;
        Table.Form = new FormInfo { Width = 500 };
        Table.Toolbar.AddAction(nameof(New));
        Table.AddColumn(c => c.Code, true).Width(100);
        Table.AddColumn(c => c.Name, true).Width(100);
        Table.AddColumn(c => c.Sort).Width(80);
        Table.AddColumn(c => c.Enabled).Width(80);
        Table.AddColumn(c => c.Note).Width(200);
        Table.AddAction(nameof(Edit));
        Table.AddAction(nameof(Delete));
    }

    public void New()
    {
        Table.NewForm(Admin.SaveDictionaryAsync, new DictionaryInfo
        {
            Category = category.Code,
            CategoryName = category.Name,
            Sort = total + 1
        });
    }

    public void Edit(DictionaryInfo row) => Table.EditForm(Admin.SaveDictionaryAsync, row);
    public void Delete(DictionaryInfo row) => Table.Delete(Admin.DeleteDictionariesAsync, row);

    private async Task<PagingResult<DictionaryInfo>> QueryDictionariesAsync(PagingCriteria criteria)
    {
        if (category == null)
            return default;

        criteria.SetQuery(nameof(DictionaryInfo.Category), QueryType.Equal, category?.Code);
        var result = await Admin.QueryDictionariesAsync(criteria);
        total = result.TotalCount;
        return result;
    }
}