using Known.Test.Pages.Samples.DataList;

namespace Known.Test.Pages.Samples;

class DemoDataList : BaseComponent
{
    private readonly TabItem[] items = new TabItem[]
    {
        new TabItem{Icon="fa fa-list",Title="综合列表",ChildContent=b => b.Component<FullList>().Build()},
        new TabItem{Icon="fa fa-table",Title="综合表格",ChildContent=b => b.Component<FullTable>().Build()},
        new TabItem{Icon="fa fa-table",Title="普通表格",ChildContent=b=>b.Component<CommonTable>().Build()},
        new TabItem{Icon="fa fa-table",Title="分页表格",ChildContent=b => b.Component<PageTable>().Build()},
        new TabItem{Icon="fa fa-table",Title="编辑表格",ChildContent=b => b.Component<EditTable>().Build()}
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Tabs>().Set(c => c.Items, items).Build();
    }
}