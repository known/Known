using WebSite.Docus.Inputs.RadioLists;

namespace WebSite.Docus.Inputs;

class KRadioList : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 支持每行显示数量
- 支持4种列表项数据源
  - 数据字典类别
  - 逗号分割的字符串
  - CodeInfo类数组
  - CodeAction函数
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<RadioList1>("1.默认示例");
        builder.BuildDemo<RadioList2>("2.控制示例");
    }
}