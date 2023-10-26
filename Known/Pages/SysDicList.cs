namespace Known.Pages;

[Route("/dictionaries")]
public class SysDicList : KDataGrid<SysDictionary, SysDicForm>
{
    private readonly List<DicCategory> dicCates = DicCategory.Categories;
    private readonly CodeInfo[] categories;
    private string category;
    private int total;

    public SysDicList()
    {
        OrderBy = $"{nameof(SysDictionary.Sort)} asc";
        categories = dicCates.Select(c => new CodeInfo(c.Name, c.Name)).ToArray();
        category = dicCates.Count > 0 ? dicCates[0].Name : "";
    }

    protected override Task InitPageAsync()
    {
        Column(c => c.Category).Select(new SelectOption
        {
            ShowEmpty = false,
            Items = categories,
            ValueChanged = v => category = v
        });
        Column(c => c.Code).Template((b, r) => b.Link(r.Code, Callback(() => View(r))));
        return base.InitPageAsync();
    }

    protected override async Task<PagingResult<SysDictionary>> OnQueryDataAsync(PagingCriteria criteria)
    {
        criteria.SetQuery(nameof(SysDictionary.Category), QueryType.Equal, category);
        var result = await Platform.Dictionary.QueryDictionarysAsync(criteria);
        total = result.TotalCount;
        return result;
    }

    public override async void Refresh()
    {
        base.Refresh();
        await Platform.Dictionary.RefreshCache();
    }

    public void New() => ShowForm();
    public void DeleteM() => DeleteRows(Platform.Dictionary.DeleteDictionarysAsync);
    public void Edit(SysDictionary row) => ShowForm(row);
    public void Delete(SysDictionary row) => DeleteRow(row, Platform.Dictionary.DeleteDictionarysAsync);
    public override void Import() => ShowImport(Name, typeof(SysDictionary), false);

    public override void View(SysDictionary row)
    {
        var dicCate = dicCates.FirstOrDefault(d => d.Name == category);
        if (dicCate == null)
            return;

        row.HasChild = dicCate.HasChild;
        if (row.HasChild)
            UI.ShowForm<SysDicForm>($"查看字典 - {category}", row, size: new Size(750, 390));
        else
            UI.ShowForm<SysDicForm>($"查看字典 - {category}", row);
    }

    protected override void ShowForm(SysDictionary model = null)
    {
        var dicCate = dicCates.FirstOrDefault(d => d.Name == category);
        if (dicCate == null)
            return;

        var action = model == null ? "新增" : "编辑";
        model ??= new SysDictionary
        {
            Category = category,
            CategoryName = category,
            Enabled = true,
            Sort = total + 1
        };
        model.HasChild = dicCate.HasChild;
        if (model.HasChild)
            ShowForm<SysDicForm>($"{action}字典 - {category}", model, new Size(750, 390));
        else
            ShowForm<SysDicForm>($"{action}字典 - {category}", model);
    }
}