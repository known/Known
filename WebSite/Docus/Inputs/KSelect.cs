using WebSite.Docus.Inputs.Selects;

namespace WebSite.Docus.Inputs;

class KSelect : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 支持4种列表项数据源
  - 数据字典类别
  - 逗号分割的字符串
  - CodeInfo类数组
  - CodeAction函数
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Select1>("1.默认示例");
        builder.BuildDemo<Select2>("2.事件示例");
        builder.BuildDemo<Select3>("3.控制示例");
    }
}