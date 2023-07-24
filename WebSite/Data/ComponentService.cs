using WebSite.Docus;
using WebSite.Docus.Basic;
using WebSite.Docus.Comprehensive;
using WebSite.Docus.Feedback;
using WebSite.Docus.Input;
using WebSite.Docus.Nav;
using WebSite.Docus.View;

namespace WebSite.Data;

class ComponentService
{
    static ComponentService()
    {
        var items = new List<MenuItem>();

        var overview = new MenuItem("overview", "概述");
        overview.Children.Add(new MenuItem("Introduction", "简介", ""));
        items.Add(overview);

        var basic = new MenuItem("basic", "基础");
        basic.Children.Add<KLayout>("布局", "用于后台主页面布局。");
        //basic.Children.Add<KGrid>("栅格", ""));
        basic.Children.Add<KIcon>("图标", "基于Fontawesome图标库。");
        items.Add(basic);

        var input = new MenuItem("input", "输入类");
        input.Children.Add<KButton>("按钮", "使用按钮触发操作，支持不同样式。");
        input.Children.Add<KInput>("输入框", "通用输入框组件。");
        input.Children.Add<KHidden>("隐藏框", "隐藏字段组件。");
        input.Children.Add<KPassword>("密码框", "密码字段组件。");
        input.Children.Add<KText>("文本框", "单行文本框输入组件。");
        input.Children.Add<KTextArea>("文本域", "多行文本框输入组件。");
        input.Children.Add<KNumber>("数值框", "数值类型输入组件。");
        input.Children.Add<KCheckBox>("复选框", "复选框组件。");
        input.Children.Add<KSwitch>("开关", "用于切换两种状态。");
        input.Children.Add<KDate>("日期框", "支持日期和时间格式。");
        input.Children.Add<KDateRange>("日期范围", "日期范围选择框组件。");
        input.Children.Add<KSelect>("选择框", "下拉选择框组件。");
        input.Children.Add<KCheckList>("复选列表", "用于选中多个选项。");
        input.Children.Add<KRadioList>("单选列表", "单选框列表组件。");
        input.Children.Add<KPicker>("挑选框", "点击按钮弹出对话框挑选数据组件。");
        input.Children.Add<KUpload>("上传", "上传文件组件。");
        input.Children.Add<KCaptcha>("验证码", "前端图片验证码组件。");
        input.Children.Add<KSearchBox>("搜索框", "带搜索按钮的输入组件。");
        items.Add(input);

        var nav = new MenuItem("nav", "导航类");
        nav.Children.Add<KMenu>("菜单", "后台主页侧边栏菜单组件。");
        nav.Children.Add<KBreadcrumb>("面包屑", "页面导航面包屑组件。");
        nav.Children.Add<KToolbar>("工具条", "一组按钮组件。");
        nav.Children.Add<KPager>("分页", "数据分页导航组件。");
        nav.Children.Add<KSteps>("步骤", "用于分步表单。");
        nav.Children.Add<KTabs>("标签栏", "用于分组显示不同内容。");
        nav.Children.Add<KTree>("树", "树型结构组件。");
        items.Add(nav);

        var view = new MenuItem("view", "展示类");
        view.Children.Add<KBadge>("徽章", "用于提示新消息。");
        view.Children.Add<KTag>("标签", "用于标记不同状态的数据。");
        view.Children.Add<KDialog>("对话框", "弹窗显示内容。");
        view.Children.Add<KCard>("卡片", "由标题和内容组成。");
        view.Children.Add<KEmpty>("空状态", "用于列表无数据时显示。");
        view.Children.Add<KDropdown>("下拉框", "用于向下弹窗菜单。");
        //view.Children.Add<KGroupBox>("分组框", "");
        view.Children.Add<KImageBox>("图片框", "用于显示图片。");
        view.Children.Add<KTimeline>("时间轴", "用于显示时间节点内容。");
        view.Children.Add<KCarousel>("走马灯", "用于轮流播放一组图片。");
        view.Children.Add<KQuickView>("快速预览", "从屏幕边缘滑出的面板。");
        view.Children.Add<KChart>("图表", "基于Highcharts.js实现。");
        view.Children.Add<KBarcode>("条形码", "基于JsBarcode实现。");
        view.Children.Add<KQRCode>("二维码", "基于jquery.qrcode实现。");
        items.Add(view);

        var feedback = new MenuItem("feedback", "反馈类");
        feedback.Children.Add<KBanner>("横幅通知", "主要用于系统级和模块级重要通知提醒。");
        feedback.Children.Add<KNotify>("通知", "主要用于系统通知，位于页面右下角，支持不同样式。");
        feedback.Children.Add<KToast>("提示", "主要用于操作提示，支持不同样式。");
        feedback.Children.Add<KProgress>("进度条", "支持不同样式。");
        items.Add(feedback);

        var comprehensive = new MenuItem("comprehensive", "综合类");
        comprehensive.Children.Add<KForm>("表单", "是一个页面级别表单布局组件。");
        comprehensive.Children.Add<KDataList>("数据列表", "支持分页、查询、列模板。");
        comprehensive.Children.Add<KDataGrid>("数据表格", "是一个集工具条、查询条件、表格、分页、操作等综合性的页面级组件。");
        comprehensive.Children.Add<KEditGrid>("编辑表格", "可编辑的数据表格，继承自DataGrid。");
        items.Add(comprehensive);

        Menus = items;
    }

    internal static List<MenuItem> Menus { get; }

    internal static RenderFragment Render(string? id)
    {
        return builder =>
        {
            MenuItem? menu = null;
            foreach (var item in Menus)
            {
                menu = item.Children.FirstOrDefault(m => m.Id == id);
                if (menu != null)
                    break;
            }
            if (menu == null)
                return;

            if (menu.Id == "Introduction")
                BuildIntroduction(builder);
            else if (menu.Type != null)
                builder.Component(menu.Type, attr => attr.Add(nameof(BaseDocu.Item), menu));
        };
    }

    private static void BuildIntroduction(RenderTreeBuilder builder)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# 简介");
        sb.AppendLine();
        sb.AppendLine("Known框架内置一套基于Blazor的UI组件，位于Known.Razor类库中。");
        sb.AppendLine();
        foreach (var item in Menus)
        {
            if (item.Id == "overview")
                continue;

            sb.AppendLine($"## {item.Name}");
            sb.AppendLine();
            foreach (var sub in item.Children)
            {
                sb.AppendLine($"- {sub.Name} ({sub.Id})：{sub.Description}");
            }
            sb.AppendLine();
        }
        var html = Markdown.ToHtml(sb.ToString());
        builder.Markup(html);
    }
}