namespace Known.Helpers;

static class InitHelper
{
    private static readonly Dictionary<string, InitInfo> Inits = [];
    private record InitInfo(Assembly Assembly, Type[] Types);

    internal static void Add(Assembly assembly)
    {
        if (assembly == null)
            return;

        if (Inits.ContainsKey(assembly.FullName))
            return;

        var types = assembly.GetTypes();
        Inits[assembly.FullName] = new InitInfo(assembly, types);

        AddActions(assembly);

        foreach (var type in types)
        {
            var attributes = type.GetCustomAttributes(false);
            var route = attributes.OfType<RouteAttribute>().FirstOrDefault();
            var menu = attributes.OfType<AppMenuAttribute>().FirstOrDefault();
            var plugin = attributes.OfType<PluginAttribute>().FirstOrDefault();
            var code = attributes.OfType<CodeInfoAttribute>().FirstOrDefault();

            if (menu != null)
                AddAppMenu(type, menu, route);
            else if (plugin != null)
                PluginConfig.AddPlugin(type, plugin, route);
            else if (code != null)
                Cache.AttachCodes(type);
            else if (type.IsEnum)
                AddEnum(type);
            else if (type.IsAssignableTo(typeof(ICustomField)) && type.Name != nameof(ICustomField) && type.Name != nameof(CustomField))
                Config.FieldTypes[type.Name] = type;
            else if (type.IsAssignableTo(typeof(BaseForm)))
                Config.FormTypes[type.Name] = type;
        }

        MigrateHelper.TopNavs = PluginConfig.LoadTopNavs();
    }

    internal static void LoadClients(this IServiceCollection services)
    {
        foreach (var item in Inits.Values)
        {
            foreach (var type in item.Types)
            {
                var attr = type.GetCustomAttribute<ClientAttribute>();
                if (attr == null)
                    continue;

                services.AddServices(attr.Lifetime, type);
            }
        }
    }

    internal static void LoadServers(this IServiceCollection services)
    {
        foreach (var item in Inits.Values)
        {
            var xml = GetAssemblyXml(item.Assembly);
            var doc = new XmlDocument();
            if (!string.IsNullOrWhiteSpace(xml))
                doc.LoadXml(xml);

            foreach (var type in item.Types)
            {
                if (type.IsAbstract)
                    continue;

                services.LoadType(doc, type);
            }
        }
    }

    private static void AddActions(Assembly assembly)
    {
        var content = Utils.GetResource(assembly, "actions");
        if (string.IsNullOrWhiteSpace(content))
            return;

        var lines = content.Split([.. Environment.NewLine]);
        if (lines == null || lines.Length == 0)
            return;

        foreach (var item in lines)
        {
            if (string.IsNullOrWhiteSpace(item) || item.StartsWith("按钮编码"))
                continue;

            var values = item.Split('|');
            if (values.Length < 2)
                continue;

            var id = values[0].Trim();
            var info = Config.Actions.FirstOrDefault(i => i.Id == id);
            if (info == null)
            {
                info = new ActionInfo { Id = id };
                Config.Actions.Add(info);
            }
            if (values.Length > 1) info.Name = values[1].Trim();
            if (values.Length > 2) info.Icon = values[2].Trim();
            if (values.Length > 3) info.Style = values[3].Trim();
            if (values.Length > 4) info.Position = values[4].Trim();

            Language.DefaultDatas.Add(info.Name);
        }
    }

    private static void AddEnum(Type type)
    {
        Cache.AttachEnumCodes(type);
        Language.DefaultDatas.AddEnum(type);
    }

    private static string GetAssemblyXml(Assembly assembly)
    {
        if (Config.IsClient)
            return string.Empty;

        var fileName = assembly.ManifestModule.Name.Replace(".dll", ".xml");
        var path = Path.Combine(AppContext.BaseDirectory, fileName);
        return Utils.ReadFile(path);
    }

    private static void AddAppMenu(Type type, AppMenuAttribute menu, RouteAttribute route)
    {
        menu.Page = type;
        menu.Url = route?.Template;
        Config.AppMenus.Add(new MenuInfo
        {
            Id = type.Name,
            Name = menu.Name,
            Icon = menu.Icon,
            Url = menu.Url,
            Sort = menu.Sort,
            Target = menu.Target,
            Role = menu.Role,
            Color = menu.Color,
            BackUrl = menu.BackUrl,
            PageType = type
        });
    }
}