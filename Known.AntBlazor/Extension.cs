using Microsoft.Extensions.DependencyInjection;

namespace Known.AntBlazor;

public static class Extension
{
    public static void AddKnownAntDesign(this IServiceCollection services, Action<AntDesignOption> action = null)
    {
        //添加AntDesign
        services.AddAntDesign();
        services.AddScoped<IUIService, UIService>();

        AntConfig.Option = new AntDesignOption();
        action?.Invoke(AntConfig.Option);

        Config.AddModule(typeof(Extension).Assembly);

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
$('.kui-table .ant-table-body').not('.form-list .ant-table-body').css('height', tableHeight+'px');";
    }
}

public class AntDesignOption
{
    public bool ShowFooter { get; set; }
    public RenderFragment Footer { get; set; }
}

class AntConfig
{
    public static AntDesignOption Option { get; set; }
}