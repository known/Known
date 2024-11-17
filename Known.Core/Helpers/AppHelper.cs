namespace Known.Core.Helpers;

class AppHelper
{
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

    private static string GetConnectionPath()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(path, "Known", Config.App.Id, $"Connection.txt");
    }
}