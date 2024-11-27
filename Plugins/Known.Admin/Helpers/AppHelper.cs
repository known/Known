using System.Text;

namespace Known.Helpers;

class AppHelper
{
    internal static string GetProductKey()
    {
        var path = GetProductKeyPath();
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        return Utils.ReadFile(path);
    }

    internal static void SaveProductKey(string productKey)
    {
        var path = GetProductKeyPath();
        if (string.IsNullOrWhiteSpace(path))
            return;

        Utils.SaveFile(path, productKey);
    }

    private static string GetProductKeyPath()
    {
        if (string.IsNullOrWhiteSpace(AdminOption.Instance.ProductId))
            return string.Empty;

        if (string.IsNullOrWhiteSpace(Config.App.Id))
            return string.Empty;

        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(path, "Known", Config.App.Id, $"{Config.App.Id}.key");
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