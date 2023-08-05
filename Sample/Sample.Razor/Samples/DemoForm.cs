using Sample.Razor.Samples.Forms;

namespace Sample.Razor.Samples;

class DemoForm : BaseComponent
{
    private readonly List<MenuItem> items = new()
    {
        new MenuItem{Icon="fa fa-table",Name="表单一",ComType=typeof(DemoForm1)},
        new MenuItem{Icon="fa fa-table",Name="表单二",ComType=typeof(DemoForm2)},
        new MenuItem{Icon="fa fa-table",Name="表单三",ComType=typeof(DemoForm3)},
        new MenuItem{Icon="fa fa-table",Name="表单四",ComType=typeof(DemoForm4)},
        new MenuItem{Icon="fa fa-table",Name="表单五",ComType=typeof(DemoForm5)}
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Tabs>()
               .Set(c => c.Position, PositionType.Left)
               .Set(c => c.CurItem, items[0])
               .Set(c => c.Items, items)
               .Set(c => c.Body, (b, m) => b.DynamicComponent(m.ComType))
               .Build();
    }
}