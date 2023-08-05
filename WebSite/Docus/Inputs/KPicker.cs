using WebSite.Docus.Inputs.Pickers;

namespace WebSite.Docus.Inputs;

class KPicker : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 点击弹出窗口双击选择数据
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Picker1>("1.默认示例");
        builder.BuildDemo<Picker2>("2.控制示例");
    }
}