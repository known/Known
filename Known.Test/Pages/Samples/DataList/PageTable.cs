using Known.Test.Pages.Samples.Models;

namespace Known.Test.Pages.Samples.DataList;

class PageTable : DmTestGrid
{
    public PageTable()
    {
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

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-tips-tr primary", "实际库存 = 采购数量 - 销售数量 - 进退货数量 + 销退货数量");
        base.BuildRenderTree(builder);
    }
}