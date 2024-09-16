namespace Known.Pages;

/// <summary>
/// 数据字典模块页面组件类。
/// </summary>
[StreamRendering]
[Route("/sys/dictionaries")]
public class SysDictionaryList : BaseTablePage<SysDictionary>
{
    private IDictionaryService Service;
    private List<CodeInfo> categories;
    private CodeInfo category;
    private bool isAddCategory;
    private int total;

    /// <summary>
    /// 异步初始化页面。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnPageInitAsync()
    {
        await base.OnPageInitAsync();
        Service = await CreateServiceAsync<IDictionaryService>();

        Table.FormTitle = row => $"{PageName} - {row.CategoryName}";
        Table.RowKey = r => r.Id;
        Table.OnQuery = QueryDictionarysAsync;
        Table.Column(c => c.Category).Template((b, r) => b.Text(r.CategoryName));
    }

    /// <summary>
    /// 页面呈现后，调用后台数据。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
            await LoadCategoriesAsync();
    }

    /// <summary>
    /// 构建页面内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Div("kui-row-28", () =>
        {
            builder.Div("kui-card kui-dictionary", () => BuildListBox(builder));
            builder.Div(() => base.BuildPage(builder));
        });
    }

    /// <summary>
    /// 异步刷新页面。
    /// </summary>
    /// <returns></returns>
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
    public void AddCategory()
    {
        isAddCategory = true;
        var model = new DialogModel
        {
            Title = Language.GetString("Button.AddCategory"),
            Width = 800,
            Content = b => b.Component<CategoryGrid>()
                            .Set(c => c.Service, Service)
                            .Set(c => c.OnRefresh, RefreshAsync)
                            .Build()
        };
        UI.ShowDialog(model);
    }

    /// <summary>
    /// 弹出新增表单对话框。
    /// </summary>
    public void New() => NewForm(category, total);

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    public void DeleteM() => Table.DeleteM(Service.DeleteDictionariesAsync);

    /// <summary>
    /// 弹出编辑表单对话框。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Edit(SysDictionary row) => Table.EditForm(Service.SaveDictionaryAsync, row);

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    public void Delete(SysDictionary row) => Table.Delete(Service.DeleteDictionariesAsync, row);

    /// <summary>
    /// 弹出数据导入对话框。
    /// </summary>
    public async void Import() => await ShowImportAsync();

    private void BuildListBox(RenderTreeBuilder builder)
    {
        builder.Component<KListBox>()
               .Set(c => c.ShowSearch, true)
               .Set(c => c.DataSource, categories)
               .Set(c => c.ItemTemplate, ItemTemplate)
               .Set(c => c.OnItemClick, OnItemClick)
               .Build();
    }

    private RenderFragment ItemTemplate(CodeInfo info) => b => b.Text($"{info.Name} ({info.Code})");

    private Task OnItemClick(CodeInfo info)
    {
        category = info;
        return Table.RefreshAsync();
    }

    private async Task<PagingResult<SysDictionary>> QueryDictionarysAsync(PagingCriteria criteria)
    {
        if (category == null)
            return default;

        criteria.SetQuery(nameof(SysDictionary.Category), QueryType.Equal, category?.Code);
        var result = await Service.QueryDictionariesAsync(criteria);
        total = result.TotalCount;
        return result;
    }

    private async Task LoadCategoriesAsync()
    {
        categories = await Service.GetCategoriesAsync();
        category ??= categories?.FirstOrDefault();
        await OnItemClick(category);
        await StateChangedAsync();
    }

    private void NewForm(CodeInfo info, int sort)
    {
        isAddCategory = false;
        Table.NewForm(Service.SaveDictionaryAsync, new SysDictionary
        {
            Category = info?.Code,
            CategoryName = info?.Name,
            Sort = sort + 1
        });
    }
}

class CategoryGrid : BaseTable<SysDictionary>
{
    private readonly CodeInfo category = new(Constants.DicCategory, Constants.DicCategory, Constants.DicCategory, null);
    private int total;

    [Parameter] public IDictionaryService Service { get; set; }
    [Parameter] public Func<Task> OnRefresh { get; set; }

    public override async Task RefreshAsync()
    {
        await base.RefreshAsync();
        await OnRefresh?.Invoke();
    }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
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
        Table.NewForm(Service.SaveDictionaryAsync, new SysDictionary
        {
            Category = category.Code,
            CategoryName = category.Name,
            Sort = total + 1
        });
    }

    public void Edit(SysDictionary row) => Table.EditForm(Service.SaveDictionaryAsync, row);
    public void Delete(SysDictionary row) => Table.Delete(Service.DeleteDictionariesAsync, row);

    private async Task<PagingResult<SysDictionary>> QueryDictionariesAsync(PagingCriteria criteria)
    {
        if (category == null)
            return default;

        criteria.SetQuery(nameof(SysDictionary.Category), QueryType.Equal, category?.Code);
        var result = await Service.QueryDictionariesAsync(criteria);
        total = result.TotalCount;
        return result;
    }
}