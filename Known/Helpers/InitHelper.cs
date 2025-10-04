namespace Known.Helpers;

static class InitHelper
{
    private static readonly Dictionary<string, Assembly> Inits = [];

    internal static void Add(Assembly assembly)
    {
        if (assembly == null)
            return;

        if (Inits.ContainsKey(assembly.FullName))
            return;

        Inits[assembly.FullName] = assembly;

        AddActions(assembly);
    }

    internal static void LoadClients(this IServiceCollection services)
    {
        Parallel.ForEach(Inits.Values, item =>
        {
            var modelTypes = new List<Type>();
            var types = item.GetTypes();
            foreach (var type in types)
            {
                if (IsIgnoreType(type))
                    continue;

                var attributes = type.GetCustomAttributes(false);
                LoadCommon(type, attributes, modelTypes);

                var attr = attributes.OfType<ClientAttribute>().FirstOrDefault();
                if (attr != null)
                    services.AddServices(attr.Lifetime, type);
            }
            TypeCache.PreloadTypes(modelTypes);
            MigrateHelper.TopNavs = PluginConfig.LoadTopNavs();
        });
    }

    internal static void LoadServers(this IServiceCollection services)
    {
        Parallel.ForEach(Inits.Values, item =>
        {
            var xml = GetAssemblyXml(item);
            var doc = new XmlDocument();
            if (!string.IsNullOrWhiteSpace(xml))
                doc.LoadXml(xml);

            var modelTypes = new List<Type>();
            var types = item.GetTypes();
            foreach (var type in types)
            {
                if (IsIgnoreType(type))
                    continue;

                var attributes = type.GetCustomAttributes(false);
                LoadCommon(type, attributes, modelTypes);
                services.LoadType(doc, type, attributes);
            }
            TypeCache.PreloadTypes(modelTypes);
            MigrateHelper.TopNavs = PluginConfig.LoadTopNavs();
        });
    }

    private static bool IsIgnoreType(Type type)
    {
        return type.IsAbstract || type.Name.EndsWith("Extension");
    }

    private static void LoadCommon(Type type, object[] attributes, List<Type> modelTypes)
    {
        var route = attributes.OfType<RouteAttribute>().FirstOrDefault();
        var menu = attributes.OfType<AppMenuAttribute>().FirstOrDefault();
        var plugin = attributes.OfType<PluginAttribute>().FirstOrDefault();
        var code = attributes.OfType<CodeInfoAttribute>().FirstOrDefault();

        if (type.IsAssignableTo(typeof(EntityBase)) || type.Name.EndsWith("Info"))
            modelTypes.Add(type);
        else if (menu != null)
            AddAppMenu(type, menu, route);
        else if (plugin != null)
            PluginConfig.AddPlugin(type, plugin, route);
        else if (code != null)
            Cache.AttachCodes(type);
        else if (type.IsEnum)
            Cache.AttachEnumCodes(type);
        else if (type.IsAssignableTo(typeof(ICustomField)) && type.Name != nameof(ICustomField) && type.Name != nameof(CustomField))
            Config.FieldTypes[type.Name] = type;
        else if (type.IsAssignableTo(typeof(BaseForm)))
            Config.FormTypes[type.Name] = type;
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