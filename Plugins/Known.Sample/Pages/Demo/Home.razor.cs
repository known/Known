namespace Known.Sample.Pages.Demo;

public partial class Home
{
    [Action]
    public void New()
    {
        var model = new DialogModel
        {
            Title = "新增",
            Style = "width:50vw"
        };
        UI.ShowDialog(model);
    }

    [Action] public void Edit() { }
    [Action] public void TestItem() { }

    private Task OnFullScreen()
    {
        return UI.NoticeAsync("全屏", "全屏打开");
    }
}