namespace Known.Razor.Components;

class Pager : BaseComponent
{
    [Parameter] public int TotalCount { get; set; }
    [Parameter] public int PageSize { get; set; }
    [Parameter] public int PageIndex { get; set; }
    [Parameter] public Func<PagingCriteria, Task> OnPageChanged { get; set; }

    public int PageCount => (int)Math.Ceiling(TotalCount * 1.0 / PageSize);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("pager", attr =>
        {
            if (Context.IsMobile)
                BuildAppPager(builder);
            else
                BuildWebPager(builder);
        });
    }

    private void BuildAppPager(RenderTreeBuilder builder)
    {
        builder.Ul("btns", attr =>
        {
            BuildPageButton(builder, Language.PagerPrevious);
            builder.Li("text", attr => builder.Text($"{PageIndex}/{PageCount}"));
            BuildPageButton(builder, Language.PagerNext);
        });
    }

    private void BuildWebPager(RenderTreeBuilder builder)
    {
        builder.Ul(attr =>
        {
            BuildPageButton(builder, Language.PagerPrevious, "fa fa-chevron-left");
            BuildPages(builder);
            BuildPageButton(builder, Language.PagerNext, "fa fa-chevron-right");
            builder.Li(attr => BuildGoPage(builder));
            builder.Li(attr => builder.Text($"共{TotalCount}条"));
            builder.Li(attr => BuildPageSize(builder));
            //builder.Li("btn fa fa-refresh", attr =>
            //{
            //    attr.Title(Language.PagerRefresh).OnClick(Callback(async e => await OnRefresh()));
            //});
        });
    }

    private void BuildPages(RenderTreeBuilder builder)
    {
        for (int i = 1; i <= PageCount; i++)
        {
            if (i == 2 && PageIndex - i > 2)
            {
                i = PageIndex - 3;
                builder.Li(attr => builder.Text("..."));
            }
            else if (i - PageIndex > 2 && i + 3 < PageCount)
            {
                i = PageCount - 2;
                builder.Li(attr => builder.Text("..."));
            }
            else
            {
                BuildPageButton(builder, i);
            }
        }
    }

    private void BuildGoPage(RenderTreeBuilder builder)
    {
        builder.Text("第");
        builder.Input(attr =>
        {
            attr.Type("text").Value($"{PageIndex}")
                .OnChange(EventCallback.Factory.CreateBinder(this, value => PageIndex = value, PageIndex));
        });
        builder.Text("页");
        builder.Button("确定", Callback(async e => await OnRefresh()));
    }

    private void BuildPageSize(RenderTreeBuilder builder)
    {
        var sizes = PagingCriteria.PageSizes;
        builder.Select(attr =>
        {
            attr.OnChange(EventCallback.Factory.CreateBinder(this, value =>
            {
                PageIndex = 1;
                PageSize = value;
                OnRefresh();
            }, PageSize));
            foreach (var size in sizes)
            {
                builder.Option(attr =>
                {
                    attr.Value($"{size}").Selected(size == PageSize);
                    builder.Text($"{size}条/页");
                });
            }
        });
    }

    private void BuildPageButton(RenderTreeBuilder builder, int pageIndex)
    {
        var css = CssBuilder.Default("btn").AddClass("active", pageIndex == PageIndex).Build();
        builder.Li(css, attr =>
        {
            attr.OnClick(Callback(async e => await PageChanged(pageIndex)));
            builder.Text($"{pageIndex}");
        });
    }

    private void BuildPageButton(RenderTreeBuilder builder, string text, string icon = null)
    {
        var disabled = IsDisabled(text);
        var css = CssBuilder.Default("btn").AddClass("disabled", disabled).AddClass(icon).Build();
        builder.Li(css, attr =>
        {
            if (!disabled)
                attr.OnClick(Callback(async e => await PageChanged(text)));

            if (!string.IsNullOrWhiteSpace(icon))
                attr.Title(text);
            else
                builder.Text(text);
        });
    }

    private bool IsDisabled(string text)
    {
        switch (text)
        {
            case Language.PagerFirst:
            case Language.PagerPrevious:
                return PageIndex == 1;
            case Language.PagerNext:
            case Language.PagerLast:
                return PageCount <= 1 || PageIndex == PageCount && PageCount > 1;
            default:
                return false;
        }
    }

    private Task PageChanged(int pageIndex)
    {
        PageIndex = pageIndex;
        return OnRefresh();
    }

    private Task PageChanged(string text)
    {
        switch (text)
        {
            case Language.PagerFirst:
                PageIndex = 1;
                break;
            case Language.PagerPrevious:
                if (PageIndex > 1)
                    PageIndex--;
                break;
            case Language.PagerNext:
                if (PageIndex < PageCount)
                    PageIndex++;
                break;
            case Language.PagerLast:
                PageIndex = PageCount;
                break;
        }
        return OnRefresh();
    }

    private Task OnRefresh()
    {
        return OnPageChanged?.Invoke(new PagingCriteria
        {
            PageIndex = PageIndex,
            PageSize = PageSize
        });
    }
}