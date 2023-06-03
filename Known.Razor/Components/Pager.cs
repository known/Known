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
                BuildPager(builder);
        });
    }

    private void BuildAppPager(RenderTreeBuilder builder)
    {
        builder.Ul("btns", attr =>
        {
            BuildPageButton(builder, Language.PagerPrevious);
            builder.Li("text", attr =>
            {
                builder.Text($"{PageIndex}/{PageCount}");
            });
            BuildPageButton(builder, Language.PagerNext);
        });
    }

    private void BuildPager(RenderTreeBuilder builder)
    {
        var total = Language.PagerTotalText.Format(TotalCount);
        builder.Span("total", total);

        builder.Span("size", attr =>
        {
            builder.Text("每页");
            BuildPageSize(builder);
            builder.Text("条");
        });

        builder.Ul(attr =>
        {
            builder.Li(attr =>
            {
                attr.Title(Language.PagerRefresh).OnClick(Callback(e => OnRefresh()));
                builder.Icon("fa fa-refresh");
            });
            BuildPageButton(builder, Language.PagerFirst, "fa fa-step-backward");
            BuildPageButton(builder, Language.PagerPrevious, "caret pp fa fa-caret-left");
            builder.Li("text", attr =>
            {
                builder.Text($"{PageIndex}/{PageCount}");
            });
            BuildPageButton(builder, Language.PagerNext, "caret pn fa fa-caret-right");
            BuildPageButton(builder, Language.PagerLast, "fa fa-step-forward");
        });
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
                    builder.Text($"{size}");
                });
            }
        });
    }

    private void BuildPageButton(RenderTreeBuilder builder, string text, string icon = null)
    {
        builder.Li(attr =>
        {
            if (IsDisabled(text))
                attr.Class("disabled");
            else
                attr.OnClick(Callback(async e => await OnChangePage(text)));

            if (!string.IsNullOrWhiteSpace(icon))
            {
                attr.Title(text);
                builder.Icon(icon);
            }
            else
            {
                builder.Text(text);
            }
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

    private Task OnChangePage(string text)
    {
        SetPageIndex(text);
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

    private void SetPageIndex(string text)
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
    }
}