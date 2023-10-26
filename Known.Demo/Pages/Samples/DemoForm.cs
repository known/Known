using Known.Demo.Pages.Samples.Forms;

namespace Known.Demo.Pages.Samples;

class DemoForm : BaseComponent
{
    private readonly List<KMenuItem> items = new()
    {
        new KMenuItem{Icon="fa fa-table",Name="表单一",ComType=typeof(DemoForm1)},
        new KMenuItem{Icon="fa fa-table",Name="表单二",ComType=typeof(DemoForm2)},
        new KMenuItem{Icon="fa fa-table",Name="表单三",ComType=typeof(DemoForm3)},
        new KMenuItem{Icon="fa fa-table",Name="表单四",ComType=typeof(DemoForm4)},
        new KMenuItem{Icon="fa fa-table",Name="表单五",ComType=typeof(DemoForm5)}
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