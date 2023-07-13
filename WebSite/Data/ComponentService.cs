namespace WebSite.Data;

class ComponentService
{
    internal static List<MenuItem> GetMenus()
    {
        var items = new List<MenuItem>();

        var overview = new MenuItem("overview", "概述");
        overview.Children.Add(new MenuItem("Profile", "简介"));
        items.Add(overview);

        //- 输入类：Button、Select、Text、TextArea、CheckBox、Switch、Hidden、Input、Password、Captcha、Date、DateRange、Number、CheckList、RadioList、Picker、Upload、SearchBox
        var input = new MenuItem("overview", "输入类");
        input.Children.Add(new MenuItem("Button", "按钮"));
        input.Children.Add(new MenuItem("Text", "文本框"));
        input.Children.Add(new MenuItem("Select", "选择框"));
        input.Children.Add(new MenuItem("TextArea", "文本域"));
        items.Add(input);

        //- 导航类：Breadcrumb、Pager、Steps、Tabs、Tree、Menu
        var nav = new MenuItem("overview", "导航类");
        nav.Children.Add(new MenuItem("Breadcrumb", "面包屑"));
        nav.Children.Add(new MenuItem("Pager", "分页"));
        nav.Children.Add(new MenuItem("Steps", "步骤"));
        nav.Children.Add(new MenuItem("Tabs", "标签"));
        nav.Children.Add(new MenuItem("Tree", "树"));
        nav.Children.Add(new MenuItem("Menu", "菜单"));
        items.Add(nav);

        //- 展示类：Card、Carousel、Empty、Dropdown、GroupBox、ImageBox、Dialog、Chart、Form、QuickView、Badge、Icon、Timeline
        var view = new MenuItem("overview", "展示类");
        view.Children.Add(new MenuItem("Card", "卡片"));
        view.Children.Add(new MenuItem("Carousel", "走马灯"));
        view.Children.Add(new MenuItem("Empty", "空状态"));
        items.Add(view);

        //- 反馈类：Toast、Notify、Banner、Progress
        var feedback = new MenuItem("overview", "反馈类");
        feedback.Children.Add(new MenuItem("Banner", "横幅通知"));
        feedback.Children.Add(new MenuItem("Progress", "进度条"));
        feedback.Children.Add(new MenuItem("Notify", "通知"));
        feedback.Children.Add(new MenuItem("Toast", "提示"));
        items.Add(feedback);

        //- 数据类：DataList、DataTable、EditTable
        return items;
    }
}