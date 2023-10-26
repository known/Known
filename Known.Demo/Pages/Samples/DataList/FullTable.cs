using Known.Demo.Pages.Samples.Models;

namespace Known.Demo.Pages.Samples.DataList;

class FullTable : DmTestGrid
{
    public FullTable()
    {
        Style = "fullTable";

        Tools = new List<ButtonInfo> { ToolButton.New, ToolButton.DeleteM, ToolButton.Import, ToolButton.Export };
        Actions = new List<ButtonInfo> { GridAction.Edit, GridAction.Delete };

        var builder = new ColumnBuilder<DmGoods>();
        builder.Field(r => r.Picture).Template((b, r) => b.Img(r.Picture));
        builder.Field(r => r.Code, true).IsVisible(false);
        builder.Field(r => r.Name, true).Template(BuildGoodsInfo);
        builder.Field(r => r.Model).IsVisible(false);
        builder.Field(r => r.Unit).Center();
        builder.Field(r => r.TaxRate).Template((b, r) => b.Text(r.TaxRate?.ToString("P")));
        builder.Field(r => r.MinQty).IsVisible(false);
        builder.Field(r => r.MaxQty).Template(BuildQtyInfo);
        builder.Field(r => r.Note);
        Columns = builder.ToColumns();
    }

    public void New() => ShowForm();
    public void DeleteM() => DeleteRows(null);
    public void Edit(DmGoods row) => ShowForm(row);
    public void Delete(DmGoods row) => DeleteRow(row, null);

    private void BuildGoodsInfo(RenderTreeBuilder builder, DmGoods row)
    {
        builder.Link(row.Name, Callback(e => View(row)));
        builder.Span("small", row.Model);
    }

    private void BuildQtyInfo(RenderTreeBuilder builder, DmGoods row)
    {
        builder.Span("qty", $"下限：{row.MinQty}");
        builder.Span("qty", $"上限：{row.MaxQty}");
    }
}