namespace Known.Razor.Pages;

public sealed class PageAction
{
    private PageAction() { }

    public static Action RefreshTheme { get; set; }
    public static Action<int> RefreshMessageCount {  get; set; }
}