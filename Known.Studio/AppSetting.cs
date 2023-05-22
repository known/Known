namespace Known.Studio;

class AppSetting
{
    internal static double ZoomFactor { get; set; } = 1;

    internal static void Load()
    {
        var path = GetSavePath();
        var json = Utils.ReadFile(path);
        var dic = Utils.FromJson<Dictionary<string, object>>(json);
        if (dic == null)
            return;

        ZoomFactor = dic.GetValue<double>("ZoomFactor");
    }

    internal static void Save()
    {
        var dic = new Dictionary<string, object>
        {
            ["ZoomFactor"] = ZoomFactor
        };
        var json = Utils.ToJson(dic);
        var path = GetSavePath();
        Utils.SaveFile(path, json);
    }

    private static string GetSavePath()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(path, "Known", "Studio", "AppSetting.data");
    }
}