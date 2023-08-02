namespace Known.Razor.Components;

public class Carousel : BaseComponent
{
    private System.Timers.Timer timer;
    private int curIndex = 0;

    [Parameter] public int Interval { get; set; } = 3000;
    [Parameter] public bool ShowSnk { get; set; } = true;
    [Parameter] public string[] Images { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        timer = new(Interval);
        timer.Elapsed += (sender, eventArgs) => OnTimerCallback();
        timer.Start();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("carousel", attr =>
        {
            BuildItems(builder);
            BuildSnks(builder);
        });
    }

    private void BuildItems(RenderTreeBuilder builder)
    {
        if (Images == null || Images.Length == 0)
            Images = new string[] { "_content/Known.Razor/img/none.png" };

        for (int i = 0; i < Images.Length; i++)
        {
            var item = Images[i];
            var css = CssBuilder.Default("carousel-item").AddClass("active animated fadeIn", i == curIndex).Build();
            builder.Div(css, attr =>
            {
                builder.Img(attr => attr.Src(item));
            });
        }
    }

    private void BuildSnks(RenderTreeBuilder builder)
    {
        if (!ShowSnk)
            return;

        if (Images == null || Images.Length == 0)
            return;

        builder.Div("carousel-snk", attr =>
        {
            for (int i = 0; i < Images.Length; i++)
            {
                var css = i == curIndex ? "active" : "";
                builder.Span(attr => attr.Class(css));
            }
        });
    }

    private void OnTimerCallback()
    {
        _ = InvokeAsync(() =>
        {
            curIndex++;
            if (curIndex >= Images.Length)
                curIndex = 0;
            StateChanged();
        });
    }
}