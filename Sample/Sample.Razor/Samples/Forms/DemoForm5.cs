using Known.Razor.Components.Fields;
using Sample.Razor.Samples.Models;

namespace Sample.Razor.Samples.Forms;

class DemoForm5 : BaseForm
{
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
            var option = new
            {
                Height = 200,
                Placeholder = "请输入通知内容"
            };
            table.Tr(attr =>
            {
                table.Field<RichText>("富文本1", "RichText1").ColSpan(5)
                     .Set(f => f.Option, option)
                     .Build();
            });
        });
    }

    public override void Load()
    {
        SetData(new
        {
            Color = "#009688",
            Email = "test@test.com",
            Range = 30,
            Tel = "1234567890",
            Search = "test",
            Url = "http://www.test.com",
            RichText1 = "<p><img src=\"Files/puman/Image/01d082276a354ec19f7141b348ad40bd.png\" style=\"max-width:100%;\"/><br/></p>",
            RichText2 = ""
        });
    }
}