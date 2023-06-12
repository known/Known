namespace Known.Test.Pages.Samples;

class DemoDataGrid : BaseComponent
{
    private readonly TabItem[] items = new TabItem[]
    {
        new TabItem{Icon="fa fa-table",Title="普通表格",ChildContent=BuildTree(b=>b.Component<Table>().Set(c => c.Type, "common").Build())},
        new TabItem{Icon="fa fa-table",Title="分页表格",ChildContent=BuildTree(b=>b.Component<Table>().Build())},
        new TabItem{Icon="fa fa-table",Title="编辑表格",ChildContent=BuildTree(b=>b.Component<EditTable>().Build())},
        new TabItem{Icon="fa fa-table",Title="综合表格",ChildContent=BuildTree(b=>b.Component<Table>().Set(c => c.Type, "all").Build())}
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Tabs>()
               .Set(c => c.Items, items)
               .Build();
    }
}

class DmGoods : EntityBase
{
    [Column("编码")] public string Code { get; set; }
    [Column("名称")] public string Name { get; set; }
    [Column("型号")] public string Model { get; set; }
    [Column("单位")] public string Unit { get; set; }
    [Column("增值税率")] public decimal? TaxRate { get; set; }
    [Column("库存下限")] public decimal? MinQty { get; set; }
    [Column("库存上限")] public decimal? MaxQty { get; set; }
    [Column("备注")] public string Note { get; set; }
}

class Table : DataGrid<DmGoods, DemoGoodsForm>
{
    public Table()
    {
        Name = "测试示例";

        var builder = new ColumnBuilder<DmGoods>();
        builder.Field(r => r.Code, true);
        builder.Field(r => r.Name, true);
        builder.Field(r => r.Model);
        builder.Field(r => r.Unit).Center();
        builder.Field(r => r.TaxRate);
        builder.Field(r => r.MinQty).IsSum();
        builder.Field(r => r.MaxQty).IsSum();
        builder.Field(r => r.Note);
        Columns = builder.ToColumns();
    }

    [Parameter] public string Type { get; set; }

    protected override void OnParametersSet()
    {
        if (Type == "common")
        {
            ShowPager = false;
            IsSort = false;
        }
        else if (Type == "all")
        {
            Tools = new List<ButtonInfo> { ToolButton.New, ToolButton.DeleteM, ToolButton.Import, ToolButton.Export };
            Actions = new List<ButtonInfo> { GridAction.View, GridAction.Edit, GridAction.Delete };
        }
    }

    protected override Task<PagingResult<DmGoods>> OnQueryData(PagingCriteria criteria)
    {
        return Task.FromResult(new PagingResult<DmGoods>
        {
            TotalCount = 220,
            PageData = GetGoodses(criteria)
        });
    }

    public void New() => ShowForm(new DmGoods());
    public void DeleteM() => OnDeleteM(null);
    public void Edit(DmGoods row) => ShowForm(row);
    public void Delete(DmGoods row) => OnDelete(row, null);

    internal static List<DmGoods> GetGoodses(PagingCriteria criteria)
    {
        var list = new List<DmGoods>();
        for (int i = 0; i < criteria.PageSize; i++)
        {
            var id = (criteria.PageIndex - 1) * criteria.PageSize + i;
            list.Add(new DmGoods
            {
                Code = $"G{id:0000}",
                Name = $"测试{criteria.PageIndex}-{i}",
                Model = $"规格型号{criteria.PageIndex}-{i}",
                Unit = "个"
            });
        }
        return list;
    }
}

class EditTable : EditGrid<DmGoods>
{
    public EditTable()
    {
        var builder = new ColumnBuilder<DmGoods>();
        builder.Field(r => r.Code, true).Edit();
        builder.Field(r => r.Name, true).Edit();
        builder.Field(r => r.Model).Edit();
        builder.Field(r => r.Unit).Center().Edit();
        builder.Field(r => r.TaxRate).Edit();
        builder.Field(r => r.MinQty).Edit();
        builder.Field(r => r.MaxQty).Edit();
        builder.Field(r => r.Note).Edit();
        Columns = builder.ToColumns();

        Data = new List<DmGoods>();
    }
}