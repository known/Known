namespace Known.Helpers;

class ClientHelper
{
    public static void ParseUserAgent(UserInfo info)
    {
        if (string.IsNullOrWhiteSpace(info.Agent))
            return;

        info.OSName = ParseOS(info.Agent);
        info.Device = ParseDevice(info.Agent);
        info.Browser = ParseBrowser(info.Agent);
    }

    private static string ParseOS(string userAgent)
    {
        if (userAgent.Contains("Windows NT 10.0")) return "Windows 10/11";
        if (userAgent.Contains("Windows NT 6.3")) return "Windows 8.1";
        if (userAgent.Contains("Windows NT 6.2")) return "Windows 8";
        if (userAgent.Contains("Windows NT 6.1")) return "Windows 7";
        if (userAgent.Contains("Mac")) return "macOS";
        if (userAgent.Contains("Linux")) return "Linux";
        if (userAgent.Contains("Android")) return "Android";
        if (userAgent.Contains("iOS") || userAgent.Contains("iPhone")) return "iOS";
        return "Unknown";
    }

    private static string ParseDevice(string userAgent)
    {
        if (userAgent.Contains("Mobile")) return "Mobile";
        if (userAgent.Contains("Tablet")) return "Tablet";
        return "Desktop";
    }

    private static string ParseBrowser(string userAgent)
    {
        if (userAgent.Contains("Edg/")) return "Edge " + ExtractVersion(userAgent, "Edg/");
        if (userAgent.Contains("Chrome/")) return "Chrome " + ExtractVersion(userAgent, "Chrome/");
        if (userAgent.Contains("Firefox/")) return "Firefox " + ExtractVersion(userAgent, "Firefox/");
        if (userAgent.Contains("Safari/") && !userAgent.Contains("Chrome")) return "Safari " + ExtractVersion(userAgent, "Safari/");
        if (userAgent.Contains("Opera/")) return "Opera " + ExtractVersion(userAgent, "Opera/");
        return "Unknown";
    }

    private static string ExtractVersion(string userAgent, string identifier)
    {
        try
        {
            int startIndex = userAgent.IndexOf(identifier) + identifier.Length;
            int endIndex = userAgent.IndexOf(' ', startIndex);
            if (endIndex == -1) endIndex = userAgent.Length;

            var versionString = userAgent.Substring(startIndex, endIndex - startIndex);
            versionString = versionString.Split(';')[0].Split(')')[0];
            return versionString;
        }
        catch
        {
            return "Unknown";
        }
    }
}