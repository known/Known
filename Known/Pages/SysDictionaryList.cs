namespace Known.Pages;

/// <summary>
/// 数据字典模块页面组件类。
/// </summary>
[Route("/sys/dictionaries")]
//[Menu(Constants.BaseData, "数据字典", "unordered-list", 2)]
[PagePlugin("数据字典", "unordered-list", PagePluginType.Module, Language.BaseData, Sort = 2)]
public class SysDictionaryList : BaseTablePage<DictionaryInfo>
{
    private KListTable<DictionaryInfo> listTable;
    private List<CodeInfo> ListData = [];
    private CodeInfo category;
    private bool isAddCategory;
    private int total;

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        Table.AdvSearch = UIConfig.IsAdvAdmin;
        Table.EnableFilter = UIConfig.IsAdvAdmin;
        Table.FormTitle = row => $"{PageName} - {row.CategoryName}";
        Table.Form = new FormInfo { Width = 600, SmallLabel = true };
        Table.FormType = typeof(DictionaryForm);
        Table.RowKey = r => r.Id;
        Table.OnQuery = QueryDictionarysAsync;
        Table.Column(c => c.Category).QueryField(false).Template((b, r) => b.Text(r.CategoryName));
        Table.Column(c => c.Sort).Filter(false);
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
               .Set(c => c.ListData, ListData)
               .Set(c => c.OnListClick, this.Callback<CodeInfo>(OnItemClickAsync))
               .Set(c => c.OnAddClick, this.Callback<MouseEventArgs>(e => AddCategory()))
               .Set(c => c.Table, Table)
               .Build(value => listTable = value);
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
            Title = Language.AddCategory,
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
    [Action]
    public void New()
    {
        if (category == null)
        {
            UI.Error(Language.TipSelectCategory);
            return;
        }

        isAddCategory = false;
        var row = new DictionaryInfo
        {
            Category = category.Code,
            CategoryName = category.Name ?? category.Code,
            Sort = total + 1,
            DicType = Utils.ConvertTo<DictionaryType>(category.Data)
        };
        Table.NewForm(Admin.SaveDictionaryAsync, row);
    }

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    [Action] public void DeleteM() => Table.DeleteM(Admin.DeleteDictionariesAsync);

    /// <summary>
    /// 弹出编辑表单对话框。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    [Action]
    public void Edit(DictionaryInfo row)
    {
        isAddCategory = false;
        Table.EditForm(Admin.SaveDictionaryAsync, row);
    }

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
        ListData = await Admin.GetCategoriesAsync();
        category = ListData?.FirstOrDefault();
        listTable?.SetListBox(ListData, category?.Code);
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
        Table.Form = new FormInfo { Width = 600, SmallLabel = true, OpenType = FormOpenType.Modal };
        Table.FormType = typeof(CategoryForm);
        Table.Toolbar.AddAction(nameof(New));
        Table.AddColumn(c => c.Code, true).Width(120);
        Table.AddColumn(c => c.Name, true).Width(150);
        Table.AddColumn(c => c.CategoryName).Width(100).Name(Language.Type).Tag();
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
            CategoryName = nameof(DictionaryType.None),
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

class CategoryForm : BaseForm<DictionaryInfo>
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model.Rows.Clear();
        Model.AddRow().AddColumn(c => c.Code).AddColumn(c => c.Name);
        Model.AddRow().AddColumn(c => c.CategoryName, c =>
        {
            c.Type = FieldType.RadioList;
            c.DisplayName = Language.Type;
            c.Category = nameof(DictionaryType);
        });
        Model.AddRow().AddColumn(c => c.Sort).AddColumn(c => c.Enabled);
        Model.AddRow().AddColumn(c => c.Note);
    }
}