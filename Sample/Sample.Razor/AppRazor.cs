using Sample.Razor.BizApply.Forms;

namespace Sample.Razor;

public sealed class AppRazor
{
    private AppRazor() { }

    public static void Initialize(bool isWeb = true)
    {
        //配置是否Web前后端分离
        KRConfig.IsWeb = isWeb;
        //注册AppJs路径
        KRConfig.AppJsPath = "./_content/Sample.Razor/script.js";
        //配置默认首页
        KRConfig.Home = new MenuItem("首页", "fa fa-home", typeof(Home));
        //注册待办事项显示流程表单
        KRConfig.ShowMyFlow = flow =>
        {
            if (flow.Flow.FlowCode == AppFlow.Apply.Code)
                ApplyForm.ShowMyFlow(flow);
        };
    }
}