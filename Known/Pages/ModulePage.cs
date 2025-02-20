﻿namespace Known.Pages;

/// <summary>
/// 系统模块管理页面组件类。
/// </summary>
[StreamRendering]
[Route("/dev/modules")]
[DevPlugin("模块管理", "appstore-add", Sort = 3)]
public class ModulePage : BasePage
{
    /// <inheritdoc />
    protected override void BuildPage(RenderTreeBuilder builder)
    {
        if (UIConfig.ModulePageType != null)
            builder.Component(UIConfig.ModulePageType);
        else
            builder.Component<ModuleList>().Build();
    }
}