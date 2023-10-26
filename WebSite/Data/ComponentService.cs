using WebSite.Docus;
using WebSite.Docus.Basic;
using WebSite.Docus.Feedback;
using WebSite.Docus.Inputs;
using WebSite.Docus.Nav;
using WebSite.Docus.Synthetical;
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
        basic.Children.Add<DLayout>("布局", "用于后台主页面布局。");
        //basic.Children.Add<DGrid>("栅格", ""));
        basic.Children.Add<DIcon>("图标", "基于Fontawesome图标库。");
        items.Add(basic);

        var input = new MenuItem("input", "输入类");
        input.Children.Add<DButton>("按钮", "使用按钮触发操作，支持不同样式。");
        input.Children.Add<DHidden>("隐藏框", "隐藏字段组件。");
        input.Children.Add<DPassword>("密码框", "密码字段组件。");
        input.Children.Add<DText>("文本框", "单行文本框输入组件。");
        input.Children.Add<DTextArea>("文本域", "多行文本框输入组件。");
        input.Children.Add<DNumber>("数值框", "数值类型输入组件。");
        input.Children.Add<DDate>("日期框", "支持日期和时间格式。");
        input.Children.Add<DDateRange>("日期范围", "日期范围选择框组件。");
        input.Children.Add<DSelect>("选择框", "下拉选择框组件。");
        input.Children.Add<DCheckBox>("复选框", "复选框及开关组件，用于切换两种状态。");
        input.Children.Add<DCheckList>("复选列表", "用于选中多个选项。");
        input.Children.Add<DRadioList>("单选列表", "单选框列表组件。");
        input.Children.Add<DPicker>("挑选框", "点击按钮弹出对话框挑选数据组件。");
        input.Children.Add<DInput>("输入框", "通用输入框组件。");
        input.Children.Add<DUpload>("上传", "上传文件组件。");
        input.Children.Add<DCaptcha>("验证码", "前端图片验证码组件。");
        input.Children.Add<DSearchBox>("搜索框", "带搜索按钮的输入组件。");
        input.Children.Add<DRichText>("富文本", "基于wangEditor.js实现。");
        items.Add(input);

        var nav = new MenuItem("nav", "导航类");
        nav.Children.Add<DMenu>("菜单", "后台主页侧边栏菜单组件。");
        nav.Children.Add<DBreadcrumb>("面包屑", "页面导航面包屑组件。");
        nav.Children.Add<DToolbar>("工具条", "一组按钮组件。");
        nav.Children.Add<DPager>("分页", "数据分页导航组件。");
        nav.Children.Add<DSteps>("步骤", "用于分步表单。");
        nav.Children.Add<DTabs>("标签栏", "用于分组显示不同内容。");
        nav.Children.Add<DTree>("树", "树型结构组件。");
        items.Add(nav);

        var view = new MenuItem("view", "展示类");
        view.Children.Add<DBadge>("徽章", "用于提示新消息。");
        view.Children.Add<DTag>("标签", "用于标记不同状态的数据。");
        view.Children.Add<DDialog>("对话框", "弹窗显示内容。");
        view.Children.Add<DCard>("卡片", "由标题和内容组成。");
        view.Children.Add<DEmpty>("空状态", "用于无数据提示。");
        view.Children.Add<DDropdown>("下拉框", "用于向下弹出内容。");
        view.Children.Add<DGroupBox>("分组框", "用于显示分组内容。");
        view.Children.Add<DImageBox>("图片框", "用于显示图片。");
        view.Children.Add<DTimeline>("时间轴", "用于显示时间节点内容。");
        view.Children.Add<DCarousel>("走马灯", "用于轮流播放一组图片。");
        view.Children.Add<DQuickView>("快速预览", "从屏幕边缘滑出的面板。");
        view.Children.Add<DChart>("图表", "基于Highcharts.js实现。");
        view.Children.Add<DBarcode>("条形码", "基于JsBarcode实现。");
        view.Children.Add<DQRCode>("二维码", "基于jquery.qrcode实现。");
        view.Children.Add<DPdfView>("PDF预览", "基于pdfobject.js实现。");
        view.Children.Add<DPrint>("打印", "打印自定义表单。");
        items.Add(view);

        var feedback = new MenuItem("feedback", "反馈类");
        feedback.Children.Add<DLoading>("加载", "主要用于耗时操作等待提示。");
        feedback.Children.Add<DBanner>("横幅通知", "主要用于系统级和模块级重要通知提醒。");
        feedback.Children.Add<DNotify>("通知", "主要用于系统通知，位于页面右下角，支持不同样式。");
        feedback.Children.Add<DToast>("提示", "主要用于操作提示，支持不同样式。");
        feedback.Children.Add<DProgress>("进度条", "支持不同样式。");
        items.Add(feedback);

        var synthetical = new MenuItem("comprehensive", "综合类");
        synthetical.Children.Add<DForm>("表单", "是一个页面级别表单布局组件。");
        synthetical.Children.Add<DDataList>("数据列表", "支持分页、查询、列模板。");
        synthetical.Children.Add<DDataGrid>("数据表格", "是一个集工具条、查询条件、表格、分页、操作等综合性的页面级组件。");
        synthetical.Children.Add<DEditGrid>("编辑表格", "可编辑的数据表格，继承自DataGrid。");
        items.Add(synthetical);

        Menus = items;
    }

    internal static List<MenuItem> Menus { get; }

    internal static string GetCode(Type type)
    {
        if (type == null)
            return string.Empty;

        var file = type.FullName.Replace("WebSite.", "").Replace('.', '\\');
        var path = Path.Combine(AppConfig.RootPath, $"{file}.cs");
        if (!File.Exists(path))
            return string.Empty;

        return File.ReadAllText(path);
    }

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
        sb.AppendLine("Known框架内置一套基于Blazor的UI组件。");
        sb.AppendLine();
        foreach (var item in Menus)
        {
            if (item.Id == "overview")
                continue;

            sb.AppendLine($"## {item.Name}");
            sb.AppendLine();
            foreach (var sub in item.Children)
            {
                sb.AppendLine($"- [{sub.Name} ({sub.Id})](/component/{sub.Id})：{sub.Description}");
            }
            sb.AppendLine();
        }
        var html = Markdown.ToHtml(sb.ToString());
        builder.Markup(html);
    }
}