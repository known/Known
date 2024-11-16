namespace Known.Core.Helpers;

class AppHelper
{
    internal static string GetProductKey()
    {
        var path = GetProductKeyPath();
        return Utils.ReadFile(path);
    }

    internal static void SaveProductKey(string productKey)
    {
        var path = GetProductKeyPath();
        Utils.SaveFile(path, productKey);
    }

    private static string GetProductKeyPath()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(path, "Known", Config.App.Id, $"{Config.App.Id}.key");
    }

    internal static void LoadConnections()
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

    internal static void SetConnections(List<DatabaseInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return;

        DbConfig.SetConnections(infos, items =>
        {
            var sb = new StringBuilder();
            foreach (var item in items)
            {
                sb.AppendLine(item.Name);
                sb.AppendLine(item.ConnectionString);
            }

            var path = GetConnectionPath();
            Utils.SaveFile(path, sb.ToString());
        });
    }

    private static string GetConnectionPath()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(path, "Known", Config.App.Id, $"Connection.txt");
    }
}