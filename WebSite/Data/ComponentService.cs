namespace WebSite.Data;

class ComponentService
{
    internal static List<MenuItem> GetMenus()
    {
        var items = new List<MenuItem>();

        var overview = new MenuItem("overview", "概述");
        overview.Children.Add(new MenuItem("Introduction", "简介"));
        items.Add(overview);

        var basic = new MenuItem("basic", "基础");
        basic.Children.Add(new MenuItem("Layout", "布局"));
        basic.Children.Add(new MenuItem("Grid", "栅格"));
        basic.Children.Add(new MenuItem("Icon", "图标"));
        items.Add(basic);

        var input = new MenuItem("input", "输入类");
        input.Children.Add(new MenuItem("Form", "表单"));
        input.Children.Add(new MenuItem("Button", "按钮"));
        input.Children.Add(new MenuItem("Input", "输入框"));
        input.Children.Add(new MenuItem("Hidden", "隐藏框"));
        input.Children.Add(new MenuItem("Password", "密码框"));
        input.Children.Add(new MenuItem("Text", "文本框"));
        input.Children.Add(new MenuItem("TextArea", "文本域"));
        input.Children.Add(new MenuItem("Number", "数值框"));
        input.Children.Add(new MenuItem("CheckBox", "复选框"));
        input.Children.Add(new MenuItem("Switch", "开关"));
        input.Children.Add(new MenuItem("Date", "日期框"));
        input.Children.Add(new MenuItem("DateRange", "日期范围"));
        input.Children.Add(new MenuItem("Select", "选择框"));
        input.Children.Add(new MenuItem("CheckList", "复选列表"));
        input.Children.Add(new MenuItem("RadioList", "单选列表"));
        input.Children.Add(new MenuItem("Picker", "挑选框"));
        input.Children.Add(new MenuItem("Upload", "上传"));
        input.Children.Add(new MenuItem("Captcha", "验证码"));
        input.Children.Add(new MenuItem("SearchBox", "搜索框"));
        items.Add(input);

        var nav = new MenuItem("nav", "导航类");
        nav.Children.Add(new MenuItem("Menu", "菜单"));
        nav.Children.Add(new MenuItem("Breadcrumb", "面包屑"));
        nav.Children.Add(new MenuItem("Pager", "分页"));
        nav.Children.Add(new MenuItem("Steps", "步骤"));
        nav.Children.Add(new MenuItem("Tabs", "标签栏"));
        nav.Children.Add(new MenuItem("Tree", "树"));
        items.Add(nav);

        var view = new MenuItem("view", "展示类");
        view.Children.Add(new MenuItem("Badge", "徽章"));
        view.Children.Add(new MenuItem("Tag", "标签"));
        view.Children.Add(new MenuItem("Dialog", "对话框"));
        view.Children.Add(new MenuItem("Card", "卡片"));
        view.Children.Add(new MenuItem("Empty", "空状态"));
        view.Children.Add(new MenuItem("Dropdown", "下拉框"));
        view.Children.Add(new MenuItem("GroupBox", "分组框"));
        view.Children.Add(new MenuItem("ImageBox", "图片框"));
        view.Children.Add(new MenuItem("Timeline", "时间轴"));
        view.Children.Add(new MenuItem("Carousel", "走马灯"));
        view.Children.Add(new MenuItem("QuickView", "快速预览"));
        view.Children.Add(new MenuItem("Chart", "图表"));
        items.Add(view);

        var feedback = new MenuItem("feedback", "反馈类");
        feedback.Children.Add(new MenuItem("Banner", "横幅通知"));
        feedback.Children.Add(new MenuItem("Progress", "进度条"));
        feedback.Children.Add(new MenuItem("Notify", "通知"));
        feedback.Children.Add(new MenuItem("Toast", "提示"));
        items.Add(feedback);

        var data = new MenuItem("data", "数据类");
        data.Children.Add(new MenuItem("DataList", "数据列表"));
        data.Children.Add(new MenuItem("DataGrid", "数据表格"));
        data.Children.Add(new MenuItem("EditGrid", "编辑表格"));
        items.Add(data);

        return items;
    }
}