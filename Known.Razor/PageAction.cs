namespace Known.Razor;

public sealed class PageAction
{
    private PageAction() { }

    public static Action RefreshTheme { get; set; }
    public static Action<string> RefreshAppName { get; set; }
    public static Action<int> RefreshMessageCount { get; set; }

    public static IPicker CreateUserPicker(string role) => new SysUserList(role);
}