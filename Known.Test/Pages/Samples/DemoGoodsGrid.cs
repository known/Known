namespace Known.Test.Pages.Samples;

class DemoGoodsGrid : EditGrid<DmGoods>
{
    public DemoGoodsGrid()
    {
        Name = "商品明细";

        var builder = new ColumnBuilder<DmGoods>();
        builder.Field(r => r.Code, true).Edit(OnCodeChanged);
        builder.Field(r => r.Name, true).Edit(new SelectOption<DmGoods>("测试,名称", OnNameChanged));
        builder.Field(r => r.Model).Edit(new CommonTable(), OnModelChanged);
        builder.Field(r => r.Unit);
        builder.Field(r => r.MinQty).IsSum().Edit();
        builder.Field(r => r.MaxQty).IsSum().Edit();
        builder.Field(r => r.Note).Edit();
        Columns = builder.ToColumns();
    }

    private void OnCodeChanged(DmGoods row, object value)
    {
        row.Name = "测试";
        row.Model = $"Model-{value}";
        row.Unit = "个";
        OnNameChanged(row, row.Name);
    }

    private void OnNameChanged(DmGoods row, object value)
    {
        row.MinQty = 10;
        row.MaxQty = 1000;
    }

    private void OnModelChanged(DmGoods row, object value)
    {
        var g = value as DmGoods;
        row.Note = $"{g.Code}-{g.Name}";
    }
}