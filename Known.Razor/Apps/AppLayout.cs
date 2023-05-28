namespace Known.Razor.Apps;

public class AppLayout : AppComponent
{
    private int conTop;
    private int conBottom;
    private AppMenuItem curItem;

    [Parameter] public string Style { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public bool ShowBack { get; set; }
    [Parameter] public bool ShowTopbar { get; set; }
    [Parameter] public bool ShowTabbar { get; set; }
    [Parameter] public AppMenuItem TopTool { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    protected virtual AppMenuItem[] Menus { get; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Context = new KRContext { IsMobile = true };
        conTop = ShowTopbar ? 50 : 0;
        conBottom = ShowTabbar ? 50 : 0;
        if (ShowTabbar)
        {
            var id = Navigation.Uri.Replace(Navigation.BaseUri, "");
            curItem = Menus.FirstOrDefault(m => m.Id == id);
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<CascadingValue<Context>>(attr =>
        {
            attr.Set(c => c.IsFixed, true)
                .Set(c => c.Value, Context)
                .Set(c => c.ChildContent, builder1 =>
                {
                    BuildTopbar(builder1);
                    BuildContent(builder1);
                    BuildTabbar(builder1);
                });
        });
        builder.Component<DialogContainer>().Build();
    }

    private void BuildTopbar(RenderTreeBuilder builder)
    {
        if (!ShowTopbar)
            return;

        builder.Component<Topbar>()
               .Set(c => c.ShowBack, ShowBack)
               .Set(c => c.Title, Title)
               .Set(c => c.Tool, TopTool)
               .Build();
    }

    private void BuildContent(RenderTreeBuilder builder)
    {
        var style = StyleBuilder.Default
            .Add("top", $"{conTop}px")
            .Add("bottom", $"{conBottom}px")
            .Build();
        var css = CssBuilder.Default("content").AddClass(Style).Build();
        builder.Div(css, attr =>
        {
            attr.Style(style);
            builder.Fragment(ChildContent);
        });
    }

    private void BuildTabbar(RenderTreeBuilder builder)
    {
        if (!ShowTabbar)
            return;

        builder.Component<Tabbar>()
               .Set(c => c.Items, Menus)
               .Set(c => c.CurItem, curItem)
               .Set(c => c.OnChanged, Callback<MenuItem>(OnMenuChanged))
               .Build();
    }

    private void OnMenuChanged(MenuItem menu) => Navigation.NavigateTo(menu.Id);
}