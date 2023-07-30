using Known.Test.Pages.Samples.Models;

namespace Known.Test.Pages.Samples.DataList;

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
                table.Field<Text>(f => f.Title).Build();
                table.Field<Text>(f => f.Name).Build();
                table.Field<Text>(f => f.Status).Build();
            });
            table.Tr(attr =>
            {
                table.Field<Date>(f => f.Time).Build();
                table.Field<Text>(f => f.Picture).Build();
                table.Field<Text>(f => f.Progress).Build();
            });
            table.Tr(attr =>
            {
                table.Field<TextArea>(f => f.Note).ColSpan(5).Build();
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