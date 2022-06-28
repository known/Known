/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-04-01     KnownChen
 * ------------------------------------------------------------------------------- */

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

public class Pager : BaseComponent
{
    [Parameter] public int TotalCount { get; set; }
    [Parameter] public int PageSize { get; set; } = 10;
    [Parameter] public int PageIndex { get; set; } = 1;
    [Parameter] public EventCallback<int> OnPageChanged { get; set; }

    public int PageCount => (int)Math.Ceiling(TotalCount * 1.0 / PageSize);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("pager", attr =>
        {
            if (AppContext.IsMobile)
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
        builder.Span(attr =>
        {
            var text = Language.PagerText.Format(TotalCount, PageIndex, PageCount);
            builder.Text(text);
            builder.Ul("btns", attr =>
            {
                BuildPageButton(builder, Language.PagerFirst);
                BuildPageButton(builder, Language.PagerPrevious);
                BuildPageButton(builder, Language.PagerNext);
                BuildPageButton(builder, Language.PagerLast);
            });
        });
    }

    private void BuildPageButton(RenderTreeBuilder builder, string text)
    {
        builder.Li(attr =>
        {
            attr.OnClick(Callback(e => OnChangePage(text)));
            builder.Text(text);
        });
    }

    private void OnChangePage(string btnText)
    {
        SetPageIndex(btnText);

        if (OnPageChanged.HasDelegate)
        {
            OnPageChanged.InvokeAsync(PageIndex);
        }
    }

    private void SetPageIndex(string btnText)
    {
        switch (btnText)
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
