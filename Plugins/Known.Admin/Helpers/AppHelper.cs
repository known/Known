namespace Known.Helpers;

class AppHelper
{
    internal static void SetConnections(InstallInfo info)
    {
        if (info.Databases == null || info.Databases.Count == 0)
            return;

        DbConfig.SetConnections(info.Databases, items =>
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