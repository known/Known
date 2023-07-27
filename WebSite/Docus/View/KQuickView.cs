﻿using WebSite.Docus.View.QuickViews;

namespace WebSite.Docus.View;

class KQuickView : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "从屏幕边缘滑出的面板"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<QuickView1>("1.默认示例");
    }
}