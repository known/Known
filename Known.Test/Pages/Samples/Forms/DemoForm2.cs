using Known.Test.Pages.Samples.DataList;
using Known.Test.Pages.Samples.Models;

namespace Known.Test.Pages.Samples.Forms;

class DemoForm2 : BaseForm
{
    private DmBill model;

    protected override void OnInitialized()
    {
        Style = "bill-form form2";
        model = DmBill.LoadDefault();
        Model = model;
    }

    protected override void BuildFields(FieldBuilder<DmBill> builder)
    {
        builder.Div("row", attr =>
        {
            builder.Field<Text>(f => f.BillNo).Enabled(false).Build();
            builder.Field<Date>(f => f.BillDate).Build();
            builder.Field<Select>(f => f.PayType).Set(c => c.Codes, "现金,转账,预付").Build();
        });
        builder.Div("row bill-row", attr =>
        {
            builder.Div("list", attr =>
            {
                builder.Component<GoodsGrid>().Set(c => c.Data, model.Lists).Build();
            });
            builder.Div("amount", attr =>
            {
                builder.Field<Number>(f => f.TotalAmount).Enabled(false).Build();
                builder.Field<Number>(f => f.PaidAmount).Build();
            });
        });
        builder.Div("row", attr =>
        {
            builder.Field<TextArea>(f => f.Note).Label("")
                   .Set(f => f.Placeholder, "备注信息")
                   .Build();
        });
    }

    public override void Save() => Submit(data =>
    {
        model.FillModel(data);
        formData = Utils.ToJson(model);
    });
}