using Known.Demo.Pages.Samples.Models;

namespace Known.Demo.Pages.Samples.DataList;

class TestForm : BaseForm<DmTest>
{
    private readonly List<DmGoods> data = new();

    protected override void BuildFields(FieldBuilder<DmTest> builder)
    {
        builder.Hidden(f => f.Id);
        builder.Table(table =>
        {
            table.ColGroup(10, 23, 10, 23, 10, 24);
            table.Tr(attr =>
            {
                table.Field<KText>(f => f.Title).Build();
                table.Field<KText>(f => f.Name).Build();
                table.Field<KText>(f => f.Status).Build();
            });
            table.Tr(attr =>
            {
                table.Field<KTextArea>(f => f.Note).ColSpan(5).Build();
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