namespace WebSite.Data;

class DocumentService
{
    private static readonly Dictionary<string, string> menuItems = new();

    internal static List<MenuItem> GetMenus()
    {
        var path = Path.Combine(AppConfig.RootPath, "docs");
        var infos = GetFileSystemInfos(path);
        var items = new List<MenuItem>();
        var overview = new MenuItem("overview", "概述");
        items.Add(overview);
        foreach (var info in infos)
        {
            var item = GetMenuItem(info);
            if (item == null) continue;

            if (!string.IsNullOrWhiteSpace(info.Extension))
            {
                overview.Children.Add(item);
                continue;
            }

            var subInfos = GetFileSystemInfos(info.FullName);
            items.Add(item);
            foreach (var subInfo in subInfos)
            {
                var subItem = GetMenuItem(subInfo);
                if (subItem != null)
                    item.Children.Add(subItem);
            }
        }
        return items;
    }

    internal static MarkupString GetDocHtml(string id)
    {
        if (!menuItems.ContainsKey(id))
            return new MarkupString("");

        var path = menuItems[id];
        if (!File.Exists(path))
            return new MarkupString("");

        var text = File.ReadAllText(path);
        var html = Markdown.ToHtml(text);
        return new MarkupString(html);
    }

    private static List<FileSystemInfo> GetFileSystemInfos(string path)
    {
        if (!Directory.Exists(path))
            return new List<FileSystemInfo>();

        var dir = new DirectoryInfo(path);
        var entries = dir.GetFileSystemInfos();
        if (entries == null || entries.Length == 0)
            return new List<FileSystemInfo>();

        if (path.EndsWith("Log_更新日志"))
            return entries.OrderByDescending(e => e.Name).ToList();

        return entries.OrderBy(e => e.Name).ToList();
    }

    private static MenuItem GetMenuItem(FileSystemInfo info)
    {
        var names = info.Name.Split('_');
        if (names.Length != 3)
            return null;

        var id = names[1].ToLower();
        var name = names[2];
        if (!string.IsNullOrWhiteSpace(info.Extension))
            name = name.Replace(info.Extension, "");
        var item = new MenuItem(id, name);
        menuItems[id] = info.FullName;
        return item;
    }
}