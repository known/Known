namespace Known.Test.Pages.Samples.Forms;

class DemoForm4 : Razor.Components.Form
{
    private readonly List<MenuItem> items = new()
    {
        new MenuItem{Icon="fa fa-home",Name="步骤一"},
        new MenuItem{Icon="fa fa-home",Name="步骤二"},
        new MenuItem{Icon="fa fa-home",Name="步骤三"}
    };

    protected override void BuildFields(RenderTreeBuilder builder)
    {
        builder.Div("demo-row", attr =>
        {
            attr.Style("height:300px;");
            builder.Component<Steps>()
                   .Set(c => c.Items, items)
                   .Set(c => c.OnChanged, OnChanged)
                   .Set(c => c.OnFinished, OnFinished)
                   .Set(c => c.Body, BuildStep)
                   .Build();
        });
    }

    protected override void BuildButtons(RenderTreeBuilder builder) { }

    private void BuildStep(RenderTreeBuilder builder, MenuItem item)
    {
        builder.Span(item.Name);
    }

    private void OnChanged(MenuItem item)
    {
        UI.Toast($"保存{item.Name}");
    }

    private void OnFinished()
    {
        UI.Toast("完成分步表单");
    }
}