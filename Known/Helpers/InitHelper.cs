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
            if (type.IsAssignableTo(typeof(ICustomField)) && type.Name != nameof(ICustomField) && type.Name != nameof(CustomField))
                Config.FieldTypes[type.Name] = type;
            else if (type.IsAssignableTo(typeof(BaseForm)))
                Config.FormTypes[type.Name] = type;
            else if (type.IsEnum)
                Cache.AttachEnumCodes(type);
        }
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

    internal static void LoadAssemblies(this IServiceCollection services)
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

                var attributes = type.GetCustomAttributes(false);
                var task = attributes.OfType<TaskAttribute>().FirstOrDefault();
                var code = attributes.OfType<CodeInfoAttribute>().FirstOrDefault();
                var import = attributes.OfType<ImportAttribute>().FirstOrDefault();
                var service = attributes.OfType<ServiceAttribute>().FirstOrDefault();

                if (type.IsInterface && !type.IsGenericTypeDefinition && type.IsAssignableTo(typeof(IService)) && type.Name != nameof(IService))
                    AddApiMethod(doc, type, type.Name[1..].Replace("Service", ""));
                else if (TypeHelper.IsGenericSubclass(type, typeof(EntityTablePage<>), out var arguments))
                    AddApiMethod(doc, typeof(IEntityService<>).MakeGenericType(arguments), type.Name);
                else if (service != null)
                    services.AddServices(service.Lifetime, type);
                else if (type.IsAssignableTo(typeof(EntityBase)) && type.Name != nameof(EntityBase))
                    DbConfig.Models.Add(type);
                else if (code != null)
                    Cache.AttachCodes(type);
                else if (task != null)
                    Config.TaskTypes[task.BizType] = type;
                else if (type.IsAssignableTo(typeof(ImportBase)) && type.Name != nameof(ImportBase))
                {
                    var key = import != null ? import.Type.Name : type.Name.Replace("Import", "");
                    Config.ImportTypes[key] = type;
                    services.AddScoped(type);
                }
                else if (type.IsAssignableTo(typeof(FlowBase)) && type.Name != nameof(FlowBase))
                {
                    Config.FlowTypes[type.Name] = type;
                    services.AddScoped(type);
                }

                var routes = RouteHelper.GetRoutes(type);
                PluginConfig.AddPlugin(type, routes);
                AddAppMenu(type, routes);
                //AddMenu(type, routes);
                RoleHelper.AddRole(type);
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

    private static HttpMethod GetHttpMethod(MethodInfo method)
    {
        if (!method.Name.StartsWith("Get"))
            return HttpMethod.Post;

        foreach (var item in method.GetParameters())
        {
            if (item.ParameterType.IsClass && item.ParameterType != typeof(string))
                return HttpMethod.Post;
        }
        return HttpMethod.Get;
    }

    private static void AddApiMethod(XmlDocument doc, Type type, string apiName)
    {
        Config.ApiTypes.Add(type);
        var methods = type.GetMethods();
        foreach (var method in methods)
        {
            if (method.IsPublic && method.DeclaringType?.Name == type.Name)
            {
                var info = new ApiMethodInfo();
                var name = method.Name.Replace("Async", "");
                info.Id = $"{type.Name}.{method.Name}";
                info.Route = $"/{apiName}/{name}";
                info.Description = GetMethodSummary(doc, method);
                info.HttpMethod = GetHttpMethod(method);
                info.MethodInfo = method;
                info.Parameters = method.GetParameters();
                Config.ApiMethods.Add(info);
            }
        }
    }

    private static string GetMethodSummary(XmlDocument doc, MethodInfo info)
    {
        if (doc == null)
            return string.Empty;

        var name = $"{info.DeclaringType.FullName}.{info.Name}";
        var node = doc.SelectSingleNode($"/doc/members/member[@name[starts-with(., 'M:{name}')]]/summary");
        if (node == null)
            return string.Empty;

        return node.InnerText?.Trim('\n').Trim();
    }

    private static void AddAppMenu(Type item, IEnumerable<RouteAttribute> routes)
    {
        var menu = item.GetCustomAttribute<AppMenuAttribute>();
        if (menu == null)
            return;

        menu.Page = item;
        menu.Url = routes?.FirstOrDefault()?.Template;
        Config.AppMenus.Add(new MenuInfo
        {
            Id = item.Name,
            Name = menu.Name,
            Icon = menu.Icon,
            Url = menu.Url,
            Sort = menu.Sort,
            Target = menu.Target,
            Role = menu.Role,
            Color = menu.Color,
            BackUrl = menu.BackUrl,
            PageType = item
        });
    }

    private static void AddMenu(Type item, IEnumerable<RouteAttribute> routes)
    {
        var menu = item.GetCustomAttribute<MenuAttribute>();
        if (menu == null)
            return;

        menu.Page = item;
        menu.Url = routes?.FirstOrDefault()?.Template;
        Config.Menus.Add(menu);
    }
}