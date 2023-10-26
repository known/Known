using Known.Pages;

namespace Known.Razor;

public sealed class PageAction
{
    private PageAction() { }

    internal static Action RefreshTheme { get; set; }
    internal static Action<string> RefreshAppName { get; set; }
    internal static Action<int> RefreshMessageCount { get; set; }

    public static IPicker CreateUserPicker(string role) => new SysUserList(role);
}