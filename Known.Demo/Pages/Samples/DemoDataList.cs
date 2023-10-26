using Known.Demo.Pages.Samples.DataList;

namespace Known.Demo.Pages.Samples;

class DemoDataList : BaseComponent
{
    private readonly List<KMenuItem> items = new()
    {
        new KMenuItem{Icon="fa fa-list",Name="综合列表",ComType=typeof(FullList)},
        new KMenuItem{Icon="fa fa-table",Name="综合表格",ComType=typeof(FullTable)},
        new KMenuItem{Icon="fa fa-table",Name="普通表格",ComType=typeof(CommonTable)},
        new KMenuItem{Icon="fa fa-table",Name="分页表格",ComType=typeof(PageTable)},
        new KMenuItem{Icon="fa fa-table",Name="编辑表格",ComType=typeof(EditTable)}
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KTabs>()
               .Set(c => c.Position, PositionType.Left)
               .Set(c => c.CurItem, items[0])
               .Set(c => c.Items, items)
               .Set(c => c.Body, (b, m) => b.DynamicComponent(m.ComType))
               .Build();
    }
}