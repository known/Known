using Known.Test.Pages.Samples.Models;

namespace Known.Test.Pages.Samples.DataList;

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
        OnPicked?.Invoke(item);
        UI.CloseDialog();
    }
    #endregion
}