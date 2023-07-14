using System.Text;
using Known.Razor.Extensions;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

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
        basic.Children.Add(new MenuItem("Layout", "布局", "用于后台主页面布局。"));
        //basic.Children.Add(new MenuItem("Grid", "栅格", ""));
        basic.Children.Add(new MenuItem("Icon", "图标", "基于Fontawesome图标库。"));
        items.Add(basic);

        var input = new MenuItem("input", "输入类");
        input.Children.Add(new MenuItem("Button", "按钮", "使用按钮触发操作，支持不同样式。"));
        input.Children.Add(new MenuItem("Input", "输入框", "通用输入框组件。"));
        input.Children.Add(new MenuItem("Hidden", "隐藏框", "隐藏字段组件。"));
        input.Children.Add(new MenuItem("Password", "密码框", "密码字段组件。"));
        input.Children.Add(new MenuItem("Text", "文本框", "单行文本框输入组件。"));
        input.Children.Add(new MenuItem("TextArea", "文本域", "多行文本框输入组件。"));
        input.Children.Add(new MenuItem("Number", "数值框", "数值类型输入组件。"));
        input.Children.Add(new MenuItem("CheckBox", "复选框", "复选框组件。"));
        input.Children.Add(new MenuItem("Switch", "开关", "用于切换两种状态。"));
        input.Children.Add(new MenuItem("Date", "日期框", "支持日期和时间格式。"));
        input.Children.Add(new MenuItem("DateRange", "日期范围", "日期范围选择框组件。"));
        input.Children.Add(new MenuItem("Select", "选择框", "下拉选择框组件。"));
        input.Children.Add(new MenuItem("CheckList", "复选列表", "用于选中多个选项。"));
        input.Children.Add(new MenuItem("RadioList", "单选列表", "单选框列表组件。"));
        input.Children.Add(new MenuItem("Picker", "挑选框", "点击按钮弹出对话框挑选数据组件。"));
        input.Children.Add(new MenuItem("Upload", "上传", "上传文件组件。"));
        input.Children.Add(new MenuItem("Captcha", "验证码", "前端图片验证码组件。"));
        input.Children.Add(new MenuItem("SearchBox", "搜索框", "带搜索按钮的输入组件。"));
        items.Add(input);

        var nav = new MenuItem("nav", "导航类");
        nav.Children.Add(new MenuItem("Menu", "菜单", "后台主页侧边栏菜单组件。"));
        nav.Children.Add(new MenuItem("Breadcrumb", "面包屑", "页面导航面包屑组件。"));
        nav.Children.Add(new MenuItem("Pager", "分页", "数据分页导航组件。"));
        nav.Children.Add(new MenuItem("Steps", "步骤", "用于分步表单。"));
        nav.Children.Add(new MenuItem("Tabs", "标签栏", "用于分组显示不同内容。"));
        nav.Children.Add(new MenuItem("Tree", "树", "树型结构组件。"));
        items.Add(nav);

        var view = new MenuItem("view", "展示类");
        view.Children.Add(new MenuItem("Badge", "徽章", "用于提示新消息。"));
        view.Children.Add(new MenuItem("Tag", "标签", "用于标记不同状态的数据。"));
        view.Children.Add(new MenuItem("Dialog", "对话框", "弹窗显示内容。"));
        view.Children.Add(new MenuItem("Card", "卡片", "由标题和内容组成。"));
        view.Children.Add(new MenuItem("Empty", "空状态", "用于列表无数据时显示。"));
        view.Children.Add(new MenuItem("Dropdown", "下拉框", "用于向下弹窗菜单。"));
        //view.Children.Add(new MenuItem("GroupBox", "分组框", ""));
        view.Children.Add(new MenuItem("ImageBox", "图片框", "用于显示图片。"));
        view.Children.Add(new MenuItem("Timeline", "时间轴", "用于显示时间节点内容。"));
        view.Children.Add(new MenuItem("Carousel", "走马灯", "用于轮流播放一组图片。"));
        view.Children.Add(new MenuItem("QuickView", "快速预览", "从屏幕边缘滑出的面板。"));
        view.Children.Add(new MenuItem("Chart", "图表", "基于Highcharts.js实现。"));
        items.Add(view);

        var feedback = new MenuItem("feedback", "反馈类");
        feedback.Children.Add(new MenuItem("Banner", "横幅通知", "主要用于系统级和模块级重要通知提醒。"));
        feedback.Children.Add(new MenuItem("Notify", "通知", "主要用于系统通知，位于页面右下角，支持不同样式。"));
        feedback.Children.Add(new MenuItem("Toast", "提示", "主要用于操作提示，支持不同样式。"));
        feedback.Children.Add(new MenuItem("Progress", "进度条", "支持不同样式。"));
        items.Add(feedback);

        var comprehensive = new MenuItem("comprehensive", "综合类");
        comprehensive.Children.Add(new MenuItem("Form", "表单", "是一个页面级别表单布局组件。"));
        comprehensive.Children.Add(new MenuItem("DataList", "数据列表", "支持分页、查询、列模板。"));
        comprehensive.Children.Add(new MenuItem("DataGrid", "数据表格", "是一个集工具条、查询条件、表格、分页、操作等综合性的页面级组件。"));
        comprehensive.Children.Add(new MenuItem("EditGrid", "编辑表格", "可编辑的数据表格，继承自DataGrid。"));
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

            if (menu.Id== "Introduction")
            {
                BuildIntroduction(builder);
                return;
            }

            BuildComponent(builder, menu);
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

    private static void BuildComponent(RenderTreeBuilder builder, MenuItem item)
    {
        builder.Element("h1", attr => builder.Text($"{item.Name} ({item.Id})"));
        builder.Div("doc-desc", item.Description);
    }
}