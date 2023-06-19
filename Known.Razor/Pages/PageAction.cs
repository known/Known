namespace Known.Razor.Pages;

public sealed class PageAction
{
    private PageAction() { }

    public static Action RefreshThemeColor { get; set; }
    public static Action RefreshSideColor { get; set; }
    public static Action<int> RefreshMessageCount {  get; set; }
}