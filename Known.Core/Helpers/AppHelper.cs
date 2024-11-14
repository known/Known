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

    internal static void RegisterConnections()
    {
        var connections = Config.App.Connections;
        if (connections == null || connections.Count == 0)
            return;

        LoadConnections(connections);
        foreach (var item in connections)
        {
            var key = item.DatabaseType.ToString();
            if (!DbProviderFactories.GetProviderInvariantNames().Contains(key))
            {
                DbProviderFactories.RegisterFactory(key, item.ProviderType);
            }
        }
    }

    private static void LoadConnections(List<ConnectionInfo> connections)
    {
        if (connections == null || connections.Count == 0)
            return;

        var path = GetConnectionPath();
        if (!File.Exists(path))
            return;

        var lines = File.ReadAllLines(path).ToList();
        if (lines == null || lines.Count == 0)
            return;

        foreach (var connection in connections)
        {
            if (!string.IsNullOrEmpty(connection.ConnectionString))
                continue;

            var index = lines.IndexOf(connection.Name);
            if (index >= 0 && lines.Count > index + 1)
                connection.ConnectionString = lines[index + 1];
        }
    }

    internal static void SetConnection(List<DatabaseInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return;

        foreach (var info in infos)
        {
            var conn = Config.App.GetConnection(info.Name);
            if (conn != null)
                conn.ConnectionString = info.ConnectionString;
        }

        SaveConnections(Config.App.Connections);
    }

    private static void SaveConnections(List<ConnectionInfo> connections)
    {
        if (connections == null || connections.Count == 0)
            return;

        var sb = new StringBuilder();
        foreach (var connection in connections)
        {
            sb.AppendLine(connection.Name);
            sb.AppendLine(connection.ConnectionString);
        }

        var path = GetConnectionPath();
        Utils.SaveFile(path, sb.ToString());
    }

    private static string GetConnectionPath()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(path, "Known", Config.App.Id, $"Connection.txt");
    }
}