namespace Known.Helpers;

/// <summary>
/// APP帮助者类。
/// </summary>
public class AppHelper
{
    /// <summary>
    /// 异步加载多语言数据。
    /// </summary>
    public static async Task LoadLanguagesAsync(Database db)
    {
        var isExist = await db.ExistsAsync<SysConfig>();
        if (!isExist)
            return;

        var infos = await db.GetConfigAsync<List<LanguageSettingInfo>>(Constants.KeyLanguage, true);
        if (infos == null || infos.Count == 0)
            infos = Language.GetDefaultSettings();
        Language.Settings = infos;

        var datas = await db.GetLanguagesAsync();
        if (datas == null || datas.Count == 0)
            datas = Language.DefaultDatas;
        Language.Datas = datas;
    }

    /// <summary>
    /// 加载数据库连接信息。
    /// </summary>
    public static void LoadConnections()
    {
        var path = GetConnectionPath();
        if (!File.Exists(path))
            return;

        var lines = File.ReadAllLines(path).ToList();
        if (lines == null || lines.Count == 0)
            return;

        DbConfig.LoadConnections(items =>
        {
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.ConnectionString))
                    continue;

                var index = lines.IndexOf(item.Name);
                if (index >= 0 && lines.Count > index + 1)
                    item.ConnectionString = lines[index + 1];
            }
        });
    }

    internal static void SetConnections(InstallInfo info)
    {
        if (info.Connections == null || info.Connections.Count == 0)
            return;

        DbConfig.SetConnections(info.Connections, items =>
        {
            if (info.IsDatabase)
            {
                var sb = new StringBuilder();
                foreach (var item in items)
                {
                    sb.AppendLine(item.Name);
                    sb.AppendLine(item.ConnectionString);
                }

                var path = GetConnectionPath();
                Utils.SaveFile(path, sb.ToString());
            }
        });
    }

    private static string GetConnectionPath()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(path, "Known", Config.App.Id, $"Connection.txt");
    }
}