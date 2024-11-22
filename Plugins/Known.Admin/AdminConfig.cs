namespace Known;

sealed class AdminConfig
{
    internal static bool IsAuth { get; set; } = true;
    internal static string AuthStatus { get; set; }
}