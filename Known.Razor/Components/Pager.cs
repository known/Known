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
        // < 1 2 3 ... 5 > 第_1_页 确定 共100条 [20条/页]
        builder.Ul(attr =>
        {
            BuildPrevious(builder);
            BuildPageButton(builder, 1);
            if (PageCount <= 5)
            {

            }
            for (int i = 0; i < 5; i++)
            {
                
            }
            BuildPageButton(builder, PageCount);
            BuildNext(builder);
            builder.Li(attr => BuildGoPage(builder));
            builder.Li(attr => builder.Text($"共{TotalCount}条"));
            builder.Li(attr => BuildPageSize(builder));
        });
    }

    private void BuildPrevious(RenderTreeBuilder builder)
    {
        builder.Li("btn fa fa-chevron-left", attr =>
        {
            if (PageIndex > 1)
                attr.OnClick(Callback(async e => await PageChanged(PageIndex--)));
        });
    }

    private void BuildNext(RenderTreeBuilder builder)
    {
        builder.Li("btn fa fa-chevron-right", attr =>
        {
            if (PageIndex < PageCount)
                attr.OnClick(Callback(async e => await PageChanged(PageIndex++)));
        });
    }

    private void BuildPageButton(RenderTreeBuilder builder, int pageIndex)
    {
        builder.Li("btn", attr =>
        {
            attr.OnClick(Callback(async e => await PageChanged(pageIndex)));
            builder.Text($"{pageIndex}");
        });
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

    private Task PageChanged(int pageIndex)
    {
        PageIndex = pageIndex;
        return OnRefresh();
    }

    private void BuildPager1(RenderTreeBuilder builder)
    {
        var total = Language.PagerTotalText.Format(TotalCount);
        builder.Span("total", total);

        builder.Span("size", attr =>
        {
            builder.Text("每页");
            BuildPageSize(builder);
            builder.Text("条");
        });

        builder.Ul("btns", attr =>
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