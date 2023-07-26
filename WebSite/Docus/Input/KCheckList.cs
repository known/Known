using WebSite.Docus.Input.CheckLists;

namespace WebSite.Docus.Input;

class KCheckList : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "支持每行显示数量",
            "支持4种列表项数据源",
            " - 数据字典类别",
            " - 逗号分割的字符串",
            " - CodeInfo类数组",
            " - CodeAction函数"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<CheckList1>();
        builder.BuildDemo<CheckList2>();
    }
}