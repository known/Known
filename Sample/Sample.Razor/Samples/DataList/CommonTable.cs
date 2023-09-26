using Sample.Razor.Samples.Models;

namespace Sample.Razor.Samples.DataList;

class CommonTable : DmTestGrid, IPicker
{
    public CommonTable()
    {
        ShowPager = false;
        IsSort = false;

        var builder = new ColumnBuilder<DmGoods>();
        builder.Field(r => r.Picture);
        builder.Field(r => r.Code, true);
        builder.Field(r => r.Name, true);
        builder.Field(r => r.Model);
        builder.Field(r => r.Unit).Center();
        builder.Field(r => r.TaxRate).Template((b, r) => b.Text(r.TaxRate?.ToString("P")));
        builder.Field(r => r.MinQty).IsSum();
        builder.Field(r => r.MaxQty).IsSum();
        builder.Field(r => r.Note);
        Columns = builder.ToColumns();
    }

    #region IPicker
    public string Title => "选择商品信息";
    public Size Size => new(750, 450);

    public void BuildPick(RenderTreeBuilder builder)
    {
        builder.Component<CommonTable>().Set(c => c.OnPicked, OnPicked).Build();
    }

    public override void OnRowDoubleClick(int row, DmGoods item)
    {
        OnPicked?.Invoke($"{item.Code}#{item.Name}");
        UI.CloseDialog();
    }
    #endregion

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (OnPicked == null)
            builder.Div("kui-tips-tr primary", "销售毛利 = 销货总金额 + 进退货总金额 + 库存总金额 - 进货总金额 - 销退货总金额");
        base.BuildRenderTree(builder);
    }
}