using Known.Test.Pages.Samples.DataList;
using Known.Test.Pages.Samples.Models;

namespace Known.Test.Pages.Samples.Forms;

class DemoForm3 : BaseForm<DmBill>
{
    private DmBill model;
    private string formData;

    protected override void OnInitialized()
    {
        Style = "inline bill-form";
        model = new DmBill
        {
            BillNo = $"B{DateTime.Now:yyyyMM}00001",
            BillDate = DateTime.Now,
            Lists = new List<DmGoods>()
        };
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
                builder.Component<DemoGoodsGrid>()
                   .Set(c => c.Data, model.Lists)
                   .Build();
            });
        });
        builder.Div("row", attr =>
        {
            builder.Field<TextArea>(f => f.Note).Label("")
                   .Set(f => f.Placeholder, "备注信息")
                   .Build();
        });
        builder.Div("row mt10", attr =>
        {
            builder.Field<Number>(f => f.TotalAmount).Enabled(false).Build();
            builder.Field<Number>(f => f.PaidAmount).Build();
            builder.Div("");
        });
        builder.Builder.Div("form-button", attr =>
        {
            builder.Builder.Button("保存", "fa fa-save", Callback(OnSaveData));
            builder.Builder.Button("清空", "fa fa-trash-o", Callback(Clear));
        });
        builder.Builder.Div("demo-tips", formData);
    }

    protected override void BuildButtons(RenderTreeBuilder builder) { }

    private void OnSaveData() => Submit(data =>
    {
        model.FillModel(data);
        formData = Utils.ToJson(model);
    });
}