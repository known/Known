using Microsoft.Extensions.DependencyInjection;

namespace Known.AntBlazor;

/// <summary>
/// 依赖注入扩展类。
/// </summary>
public static class Extension
{
    /// <summary>
    /// 添加基于AntDesign Blazor的实现框架界面。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">配置选项。</param>
    public static void AddKnownAntDesign(this IServiceCollection services, Action<AntDesignOption> action = null)
    {
        //添加AntDesign
        services.AddAntDesign();
        services.AddScoped<IUIService, UIService>();

        AntConfig.Option = new AntDesignOption();
        action?.Invoke(AntConfig.Option);

        Config.AddModule(typeof(Extension).Assembly);

        KStyleSheet.AddStyleSheet("_content/AntDesign/css/ant-design-blazor.css");
        KStyleSheet.AddStyleSheet("_content/Known.AntBlazor/css/web.css");
        KScript.AddScript("_content/AntDesign/js/ant-design-blazor.js");

        UIConfig.Sizes = [
            new ActionInfo { Id = "Default", Style = "ant-design-blazor", Url = "_content/AntDesign/css/ant-design-blazor.css" },
            new ActionInfo { Id = "Compact", Style = "ant-design-blazor", Url = "_content/AntDesign/css/ant-design-blazor.compact.css" }
        ];
        UIConfig.Icons["AntDesign"] = typeof(IconType.Outline).GetProperties()
            .Select(x => (string)x.GetValue(null))
            .Where(x => x is not null)
            .ToList();
        UIConfig.FillHeightScript = @"
var page = $('.kui-page').outerHeight(true) || 0;
var tab = $('.kui-page > .ant-tabs > .ant-tabs-nav').outerHeight(true) || 0;
var tabs = $('.kui-page .ant-tabs-nav').outerHeight(true) || 0;
var query = $('.kui-page .kui-query').outerHeight(true) || 0;
var toolbar = $('.kui-table .kui-toolbar').outerHeight(true) || 0;
var tableHead = $('.kui-table .ant-table-header').outerHeight(true) || 0;
var pagination = $('.kui-table .ant-table-pagination').outerHeight(true) || 0;
//console.log('page='+page+',tab='+tab+',tabs='+tabs+',query='+query+',toolbar='+toolbar+',tableHead='+tableHead+',pagination='+pagination);
var cardDiff = tab > 0 ? 40 : 10;
var cardHeight = page-tab-tabs-cardDiff;
var tableDiff = tab > 0 ? 60 : 20;
var tableHeight = page-tab-query-tabs-toolbar-tableHead-pagination-tableDiff;
$('.kui-card .ant-tabs-content-holder').css('height', cardHeight+'px');
$('.kui-card .ant-tabs-content-holder .ant-tabs-content-holder').css('height', (cardHeight-tableHead-15)+'px');
$('.kui-table .ant-table-body').not('.form-list .ant-table-body').css('height', tableHeight+'px');";
    }
}

/// <summary>
/// AntDesign配置选项类。
/// </summary>
public class AntDesignOption
{
    /// <summary>
    /// 取得或设置是否显示页面页脚组件。
    /// </summary>
    public bool ShowFooter { get; set; }

    /// <summary>
    /// 取得或设置页面页脚组件。
    /// </summary>
    public RenderFragment Footer { get; set; }
}

class AntConfig
{
    public static AntDesignOption Option { get; set; }
}