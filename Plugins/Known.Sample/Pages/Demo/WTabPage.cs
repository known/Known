using Known.Sample.Pages.Demo.Forms;

namespace Known.Sample.Pages.Demo;

[Route("/demo/tabform")]
[Menu(AppConstant.Demo, "标签页面", "form", 5)]
public class WTabPage : BaseTabPage
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        Tab.AddTab<BasicSetting>("基本设置");
        Tab.AddTab<SettingList>("列表设置", BuildSettingList);
    }

    private void BuildSettingList(RenderTreeBuilder builder)
    {
        builder.Component<SettingList>()
               .Set(c => c.Name, "测试")
               .Build();
    }

    public void ShowModal()
    {
        var model = new DialogModel
        {
            Title = "测试",
            Content = b => b.Component<SettingList>().Build()
        };
        UI.ShowDialog(model);
    }
}