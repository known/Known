namespace WebSite.Docus.Basic;

class KLayout : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "内置后台主页默认布局为Header、Sider、Body组合",
            "布局Header和Sider颜色可通过主题设置自定义",
            "Body支持单页和多标签页",
            "可自定义后台主页布局，重写Index页面的BuildAdmin方法"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<KLayout1>("默认布局", "class KLayout1 : Known.Razor.Pages.Index { }");
    }

    class KLayout1 : Known.Razor.Pages.Index
    {
        protected override Task OnInitializedAsync()
        {
            OnLogin(new Known.Models.UserInfo { Name = "管理员" });
            return base.OnInitializedAsync();
        }
    }
}