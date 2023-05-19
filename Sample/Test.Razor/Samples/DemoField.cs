namespace Test.Razor.Samples;

class DemoField : Form
{
    private readonly string Codes = "孙膑,后羿,妲己";

    public DemoField()
    {
        IsTable = true;
    }

    protected override void BuildFields(RenderTreeBuilder builder)
    {
        builder.Table(attr =>
        {
            builder.ColGroup(10, 23, 10, 23, 10, 24);
            builder.Tr(attr =>
            {
                builder.Field<Text>("文本", "Text").Build();
                builder.Field<Number>("数值", "Number").Build();
                builder.Field<Select>("下拉", "Select").Set(f => f.Codes, Codes).Build();
            });
            builder.Tr(attr =>
            {
                builder.Field<Date>("日期", "Date").Build();
                builder.Field<Date>("月份", "Month").Set(f => f.DateType, DateType.Month).Build();
                builder.Field<Date>("日期时间", "DateTime").Set(f => f.DateType, DateType.DateTime).Build();
            });
            builder.Tr(attr =>
            {
                builder.Field<RadioList>("单选", "RadioList").Set(f => f.Codes, Codes).Build();
                builder.Th("", "选项");
                builder.Td(attr =>
                {
                    builder.Field<CheckBox>("CheckBox").IsInput(true).Set(f => f.Text, "启用").Build();
                });
                builder.Field<CheckList>("多选", "CheckList").Set(f => f.Codes, Codes).Build();
            });
            builder.Tr(attr =>
            {
                builder.Field<Picker>("选择（单选）", "Picker").Build();
                builder.Field<Picker>("选择（多选）", "Picker").Build();
                builder.Field<Upload>("附件", "Upload").Build();
            });
            builder.Tr(attr => builder.Field<TextArea>("文本域", "TextArea").ColSpan(5).Build());
        });
    }
}