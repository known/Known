namespace Test.Razor.Samples;

class DemoDataGrid : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("demo-caption", "普通表格");
        builder.Div("demo-box", attr =>
        {
            attr.Style("height:300px;");
            builder.Component<Table>().Set(c => c.Type, "common").Build();
        });

        builder.Div("demo-caption", "分页表格");
        builder.Div("demo-box", attr =>
        {
            attr.Style("height:350px;");
            builder.Component<Table>().Build();
        });

        builder.Div("demo-caption", "编辑表格");
        builder.Div("demo-box", attr =>
        {
            attr.Style("height:300px;");
            builder.Component<EditTable>().Build();
        });

        builder.Div("demo-caption", "综合表格");
        builder.Div("demo-box", attr =>
        {
            attr.Style("height:350px;");
            builder.Component<Table>().Set(c => c.Type, "all").Build();
        });
    }
}

class DmGoods
{
    [Column("编码")] public string? Code { get; set; }
    [Column("名称")] public string? Name { get; set; }
    [Column("型号")] public string? Model { get; set; }
    [Column("单位")] public string? Unit { get; set; }
    [Column("增值税率")] public decimal? TaxRate { get; set; }
    [Column("库存下限")] public decimal? MinQty { get; set; }
    [Column("库存上限")] public decimal? MaxQty { get; set; }
    [Column("备注")] public string? Note { get; set; }
}

class Table : DataGrid<DmGoods>
{
    public Table()
    {
        var builder = new ColumnBuilder<DmGoods>();
        builder.Field(r => r.Code, true);
        builder.Field(r => r.Name, true);
        builder.Field(r => r.Model);
        builder.Field(r => r.Unit).Center();
        builder.Field(r => r.TaxRate);
        builder.Field(r => r.MinQty);
        builder.Field(r => r.MaxQty);
        builder.Field(r => r.Note);
        Columns = builder.ToColumns();
    }

    [Parameter] public string? Type { get; set; }

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