﻿namespace Known.Pages;

/// <summary>
/// 无代码页面组件类。
/// </summary>
[StreamRendering]
[Route("/page/{*PageRoute}")]
public class AutoPage : BaseComponent
{
    private IAutoPage page;
    private string pageRoute;
    private string PageId { get; set; }

    /// <summary>
    /// 取得或设置页面路由。
    /// </summary>
    [Parameter] public string PageRoute { get; set; }

    /// <summary>
    /// 异步设置页面参数。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParameterAsync()
    {
        await base.OnParameterAsync();
        if (pageRoute != PageRoute)
        {
            pageRoute = PageRoute;
            PageId = Context.Current?.Id;
            if (page != null)
            {
                await page.InitializeAsync();
                await Platform.AddPageLogAsync(Context);
            }
        }
    }

    /// <summary>
    /// 构建页面内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Context.Current == null)
        {
            UI.Page404(builder, PageId);
            return;
        }

        var type = Utils.ConvertTo<ModuleType>(Context.Current.Target);
        if (type == ModuleType.IFrame)
        {
            builder.IFrame(Context.Current.Url);
            return;
        }

        if (type == ModuleType.Page && UIConfig.EditPageType == null)
        {
            builder.Component<AutoTablePage>()
                   .Set(c => c.PageId, PageId)
                   .Build(value => page = value);
        }
        else
        {
            builder.DynamicComponent(UIConfig.EditPageType);
        }
    }
}