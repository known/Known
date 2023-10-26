namespace WebSite.Docus.Nav.Tabses;

class Tabs1 : BaseComponent
{
    private readonly List<KMenuItem> tabItems = new()
    {
        new KMenuItem { Icon = "fa fa-file-o", Name = "Tab1" },
        new KMenuItem { Icon = "fa fa-file-o", Name = "Tab2" }
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KTabs>()
               .Set(c => c.CurItem, tabItems[0])
               .Set(c => c.Items, tabItems)
               .Set(c => c.Body, (b, m) => b.Span($"{m.Name} Content"))
               .Build();
    }
}