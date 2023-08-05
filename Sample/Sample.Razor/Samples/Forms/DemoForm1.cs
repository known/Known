using Sample.Razor.Samples.DataList;

namespace Sample.Razor.Samples.Forms;

class DemoForm1 : BaseForm
{
    private readonly string Codes = "孙膑,后羿,妲己";

    protected override void BuildFields(RenderTreeBuilder builder)
    {
        builder.Table(table =>
        {
            table.ColGroup(11, 22, 11, 22, 11, 23);
            table.Tr(attr =>
            {
                table.Field<Text>("文本", "Text", true).Build();
                table.Field<Number>("数值", "Number").Build();
                table.Field<Select>("下拉", "Select", true).Set(f => f.Codes, Codes).Build();
            });
            table.Tr(attr =>
            {
                table.Field<Date>("日期", "Date").Build();
                table.Field<Date>("月份", "Month").Set(f => f.DateType, DateType.Month).Build();
                table.Field<Date>("日期时间", "DateTime").Set(f => f.DateType, DateType.DateTime).Build();
            });
            table.Tr(attr =>
            {
                table.Field<Date>("星期", "Week").Set(f => f.DateType, DateType.Week).Build();
                table.Field<Date>("时间", "Time").Set(f => f.DateType, DateType.Time).Build();
                table.Field<Input>("颜色", "Color").Set(f => f.Type, InputType.Color).Build();
            });
            table.Tr(attr =>
            {
                table.Field<RadioList>("单选", "RadioList").ColSpan(3).Set(f => f.Codes, Codes).Build();
                table.Field<Input>("邮箱", "Email").Set(f => f.Type, InputType.Email).Build();
            });
            table.Tr(attr =>
            {
                table.Field<CheckList>("多选", "CheckList", true).ColSpan(3).Set(f => f.Codes, Codes).Build();
                table.Field<Input>("电话号码", "Tel").Set(f => f.Type, InputType.Tel).Build();
            });
            table.Tr(attr =>
            {
                table.Field<CheckBox>("状态", "Status").Set(f => f.Switch, true).Set(f => f.Text, "启用").Build();
                table.Field<Input>("搜索", "Search").Set(f => f.Type, InputType.Search).Build();
                table.Field<Input>("URL", "Url").Set(f => f.Type, InputType.Url).Build();
            });
            table.Tr(attr =>
            {
                table.Th("", "选项");
                table.Td("inline", attr =>
                {
                    attr.ColSpan(3);
                    table.Field<CheckBox>("CheckBox1").IsInput(true).Set(f => f.Text, "启用").Build();
                    table.Field<CheckBox>("ToolVisible").ValueChanged(OnToolVisibleChanged).IsInput(true).Set(f => f.Text, "工具条按钮可见").Build();
                    table.Field<CheckBox>("ToolEnabled").ValueChanged(OnToolEnabledChanged).IsInput(true).Set(f => f.Text, "工具条按钮可用").Build();
                });
                table.Field<Input>("Slider", "Range").Set(f => f.Type, InputType.Range).Build();
            });
            table.Tr(attr =>
            {
                table.Field<Picker>("选择（单选）", "Picker1").Set(f => f.Pick, new CommonTable()).Build();
                table.Field<Picker>("选择（多选）", "Picker2").Build();
                table.Field<Upload>("附件", "Upload").Build();
            });
            table.Tr(attr => table.Field<TextArea>("文本域", "TextArea").ColSpan(5).Build());
        });
    }

    private void OnToolVisibleChanged(string value)
    {
        var check = Utils.ConvertTo<bool>(value);
        toolbar.SetItemVisible(check, "Edit", "Check");
    }

    private void OnToolEnabledChanged(string value)
    {
        var check = Utils.ConvertTo<bool>(value);
        toolbar.SetItemEnabled(check, "Edit", "Check");
    }

    public override void Load()
    {
        SetData(new
        {
            Text = "test",
            Number = 20,
            Select = "孙膑",
            Date = new DateTime(2020, 01, 01),
            Month = $"{DateTime.Now:yyyy-MM}",
            DateTime = DateTime.Now,
            Week = "2023-W30",
            Time = "11:59:00",
            Color = "#009688",
            RadioList = "后羿",
            Status = true,
            CheckBox = true,
            ToolVisible = true,
            ToolEnabled = true,
            CheckList = "孙膑,妲己",
            Picker1 = "test1",
            Picker2 = "test1,test2",
            TextArea = "Test Note",
            Email = "test@test.com",
            Tel = "1234567890",
            Search = "test",
            Url = "http://www.test.com",
            Range = 30
        });
    }
}