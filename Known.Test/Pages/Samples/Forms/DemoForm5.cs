using Known.Test.Pages.Samples.Models;

namespace Known.Test.Pages.Samples.Forms;

class DemoForm5 : BaseForm
{
    protected override void OnInitialized()
    {
    }

    protected override void BuildFields(FieldBuilder<DmBill> builder)
    {
        builder.Table(table =>
        {
            table.ColGroup(11, 22, 11, 22, 11, 23);
            table.Tr(attr =>
            {
                table.Field<Input>("颜色", "Color").Set(f => f.Type, InputType.Color).Build();
                table.Field<Input>("邮箱", "Email").Set(f => f.Type, InputType.Email).Build();
                table.Field<Input>("Slider", "Range").Set(f => f.Type, InputType.Range).Build();
            });
            table.Tr(attr =>
            {
                table.Field<Input>("搜索", "Search").Set(f => f.Type, InputType.Search).Build();
                table.Field<Input>("电话号码", "Tel").Set(f => f.Type, InputType.Tel).Build();
                table.Field<Input>("URL", "Url").Set(f => f.Type, InputType.Url).Build();
            });
            table.Tr(attr =>
            {
                table.Field<RichText>("富文本", "RichText").ColSpan(5)
                     .Set(f => f.Option, new
                     {
                         Height = 200,
                         Placeholder = "请输入通知内容"
                     })
                     .Build();
            });
        });
    }

    public override void Save() => Submit(data =>
    {
        formData = Utils.ToJson(data);
    });
}