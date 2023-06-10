namespace Known.Razor.Pages;

class PageAction
{
    internal static Action<string> RefreshThemeColor { get; set; }
    internal static Action<int> RefreshMessageCount {  get; set; }
}