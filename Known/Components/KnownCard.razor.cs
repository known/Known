namespace Known.Components;

/// <summary>
/// Known信息卡片组件类。
/// </summary>
public partial class KnownCard
{
    private string knownImgUrl;

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        var vknown = GetVersion<Config>();
        knownImgUrl = $"https://img.shields.io/badge/Known-{vknown}-blue";
    }

    private static string GetVersion<T>()
    {
        var assembly = typeof(T).Assembly;
        var version = assembly.GetName().Version;
        return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}