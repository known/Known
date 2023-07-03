using Known.Test.Pages.Samples.DataList;

namespace Known.Test.Pages.Samples;

class DemoDataList : BaseComponent
{
    private readonly List<MenuItem> items = new()
    {
        new MenuItem{Icon="fa fa-list",Name="综合列表",ComType=typeof(FullList)},
        new MenuItem{Icon="fa fa-table",Name="综合表格",ComType=typeof(FullTable)},
        new MenuItem{Icon="fa fa-table",Name="普通表格",ComType=typeof(CommonTable)},
        new MenuItem{Icon="fa fa-table",Name="分页表格",ComType=typeof(PageTable)},
        new MenuItem{Icon="fa fa-table",Name="编辑表格",ComType=typeof(EditTable)}
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Tabs>()
               .Set(c => c.CurItem, items[0])
               .Set(c => c.Items, items)
               .Set(c => c.Body, (b, m) => b.DynamicComponent(m.ComType))
               .Build();
    }
}