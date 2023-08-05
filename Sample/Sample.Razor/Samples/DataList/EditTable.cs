using Sample.Razor.Samples.Models;

namespace Sample.Razor.Samples.DataList;

class EditTable : EditGrid<DmGoods>
{
    public EditTable()
    {
        var builder = new ColumnBuilder<DmGoods>();
        builder.Field(r => r.Code, true).Edit(OnCodeChanged);
        builder.Field(r => r.Name, true).Edit(new SelectOption<DmGoods>("测试,名称", OnNameChanged));
        builder.Field(r => r.Model).Edit(new CommonTable(), OnModelChanged);
        builder.Field(r => r.Unit).Center().Edit();
        builder.Field(r => r.TaxRate).Edit();
        builder.Field(r => r.MinQty).Edit();
        builder.Field(r => r.MaxQty).Edit();
        builder.Field(r => r.Note).Edit();
        Columns = builder.ToColumns();

        Data = new List<DmGoods>();
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
        row.Model = g.Model;
        row.Note = $"{g.Code}-{g.Name}";
    }
}