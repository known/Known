using Known.Test.Pages.Samples.Models;

namespace Known.Test.Pages.Samples.Forms;

class DemoForm4 : BaseForm<DmBill>
{
    private DmBill model;
    private string formData;

    protected override void OnInitialized()
    {
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
        builder.Table(table =>
        {

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