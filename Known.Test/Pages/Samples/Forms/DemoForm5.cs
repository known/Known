using Known.Test.Pages.Samples.Models;

namespace Known.Test.Pages.Samples.Forms;

class DemoForm5 : BaseForm
{
    private DmBill model;

    protected override void OnInitialized()
    {
        model = DmBill.LoadDefault();
        Model = model;
    }

    protected override void BuildFields(FieldBuilder<DmBill> builder)
    {
        builder.Table(table =>
        {

        });
    }

    public override void Save() => Submit(data =>
    {
        model.FillModel(data);
        formData = Utils.ToJson(model);
    });
}