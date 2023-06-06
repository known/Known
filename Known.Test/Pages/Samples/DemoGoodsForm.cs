namespace Known.Test.Pages.Samples;

[Dialog(800, 550)]
class DemoGoodsForm : BaseForm<DmGoods>
{
    protected override void BuildFields(FieldBuilder<DmGoods> builder)
    {
        builder.Hidden(f => f.Id);
        builder.Table(table =>
        {
            table.ColGroup(10, 23, 10, 23, 10, 24);
            table.Tr(attr =>
            {
                builder.Field<Text>(f => f.Code).Build();
                builder.Field<Date>(f => f.Name).Build();
                builder.Field<Text>(f => f.Unit).Build();
            });
            table.Tr(attr =>
            {
                builder.Field<TextArea>(f => f.Model).ColSpan(5).Build();
            });
            table.FormList<DemoGoodsGrid>("商品明细", 6, 235, attr =>
            {
                attr.Set(c => c.ReadOnly, ReadOnly);
            });
        });
    }

    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button(FormButton.Save, Callback(OnSave), !ReadOnly);
        base.BuildButtons(builder);
    }

    private void OnSave()
    {
    }
}