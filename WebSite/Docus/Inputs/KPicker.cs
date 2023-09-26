using WebSite.Docus.Inputs.Pickers;

namespace WebSite.Docus.Inputs;

class KPicker : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 点击弹出窗口双击选择数据
- 支持ID隐藏字段和显示字段
- 支持回调对象和字符串
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Picker1>("1.默认示例");
        builder.BuildDemo<Picker2>("2.控制示例");
        builder.BuildDemo<Picker3>("3.回调分隔符示例");
        builder.BuildDemo<Picker4>("4.回调属性名示例");
    }
}