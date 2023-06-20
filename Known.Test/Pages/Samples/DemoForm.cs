using Known.Test.Pages.Samples.Forms;

namespace Known.Test.Pages.Samples;

class DemoForm : Razor.Components.Form
{
    private readonly TabItem[] items = new TabItem[]
    {
        new TabItem{Icon="fa fa-table",Title="表单一",ChildContent=b => b.Component<DemoForm1>().Build()},
        new TabItem{Icon="fa fa-table",Title="表单二",ChildContent=b => b.Component<DemoForm2>().Build()},
        new TabItem{Icon="fa fa-table",Title="表单三",ChildContent=b => b.Component<DemoForm3>().Build()},
        new TabItem{Icon="fa fa-table",Title="表单四",ChildContent=b => b.Component<DemoForm4>().Build()},
        new TabItem{Icon="fa fa-table",Title="表单五",ChildContent=b => b.Component<DemoForm5>().Build()}
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Tabs>().Set(c => c.Items, items).Build();
    }
}