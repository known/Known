namespace Known.Test.Pages.Samples;

class DemoGoodsGrid : EditGrid<DmGoods>
{
    public DemoGoodsGrid()
    {
        Name = "商品明细";

        var builder = new ColumnBuilder<DmGoods>();
        builder.Field(r => r.Code, true).Edit();
        builder.Field(r => r.Name, true);
        builder.Field(r => r.Model);
        builder.Field(r => r.Unit);
        builder.Field(r => r.MinQty).IsSum().Edit();
        builder.Field(r => r.MaxQty).IsSum().Edit();
        builder.Field(r => r.Note).Edit();
        Columns = builder.ToColumns();
    }
}