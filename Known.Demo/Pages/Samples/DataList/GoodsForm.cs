using Known.Demo.Pages.Samples.Models;

namespace Known.Demo.Pages.Samples.DataList;

[Dialog(800, 550)]
class GoodsForm : BaseForm<DmGoods>
{
    private readonly List<DmGoods> data = new();

    protected override void BuildFields(FieldBuilder<DmGoods> builder)
    {
        builder.Hidden(f => f.Id);
        builder.Table(table =>
        {
            table.ColGroup(10, 23, 10, 23, 10, 24);
            table.Tr(attr =>
            {
                table.Field<KText>(f => f.Code).Build();
                table.Field<KText>(f => f.Name).Build();
                table.Field<KText>(f => f.Unit).Build();
            });
            table.Tr(attr =>
            {
                table.Field<KTextArea>(f => f.Model).ColSpan(5).Build();
            });
        });
        builder.FormList<GoodsGrid>("商品明细", "", attr =>
        {
            attr.Set(c => c.ReadOnly, ReadOnly)
                .Set(c => c.Data, data);
        });
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button(FormButton.Save, Callback(OnSave), !ReadOnly);
        base.BuildButtons(builder);
    }

    private void OnSave()
    {
        Validate();
    }
}