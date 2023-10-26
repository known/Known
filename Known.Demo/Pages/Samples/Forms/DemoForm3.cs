using Known.Demo.Pages.Samples.DataList;
using Known.Demo.Pages.Samples.Models;

namespace Known.Demo.Pages.Samples.Forms;

class DemoForm3 : BaseForm
{
    private DmBill model;

    protected override void OnInitialized()
    {
        Style = "bill-form";
        model = DmBill.LoadDefault();
        Model = model;
    }

    protected override void BuildFields(FieldBuilder<DmBill> builder)
    {
        builder.Div("row", attr =>
        {
            builder.Field<KText>(f => f.BillNo).Enabled(false).Build();
            builder.Field<KDate>(f => f.BillDate).Build();
            builder.Field<KSelect>(f => f.PayType).Set(c => c.Codes, "现金,转账,预付").Build();
        });
        builder.Div("row bill-row", attr =>
        {
            builder.Div("list", attr =>
            {
                builder.Component<GoodsGrid>().Set(c => c.Data, model.Lists).Build();
            });
        });
        builder.Div("row", attr =>
        {
            builder.Field<KTextArea>(f => f.Note).Label("")
                   .Set(f => f.Placeholder, "备注信息")
                   .Build();
        });
        builder.Div("row mt10", attr =>
        {
            builder.Field<KNumber>(f => f.TotalAmount).Enabled(false).Build();
            builder.Field<KNumber>(f => f.PaidAmount).Build();
            builder.Div("");
        });
    }

    public override void Save() => Submit(data =>
    {
        model.FillModel(data);
        formData = Utils.ToJson(model);
    });
}