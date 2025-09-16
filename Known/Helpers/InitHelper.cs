namespace Known.Helpers;

class InitHelper
{
    private static readonly List<string> InitAssemblies = [];

    internal static void Load(Assembly assembly)
    {
        if (assembly == null)
            return;

        if (InitAssemblies.Contains(assembly.FullName))
            return;

        InitAssemblies.Add(assembly.FullName);
        AddActions(assembly);

        var xml = GetAssemblyXml(assembly);
        var doc = new XmlDocument();
        if (!string.IsNullOrWhiteSpace(xml))
            doc.LoadXml(xml);
        foreach (var item in assembly.GetTypes())
        {
            if (TypeHelper.IsGenericSubclass(item, typeof(EntityTablePage<>), out var arguments))
                AddApiMethod(doc, typeof(IEntityService<>).MakeGenericType(arguments), item.Name);
            else if (item.IsInterface && !item.IsGenericTypeDefinition && item.IsAssignableTo(typeof(IService)) && item.Name != nameof(IService))
                AddApiMethod(doc, item, item.Name[1..].Replace("Service", ""));
            else if (item.IsAssignableTo(typeof(ICustomField)))
                AddFieldType(item);
            else if (item.IsAssignableTo(typeof(BaseForm)))
                Config.FormTypes[item.Name] = item;
            else if (item.IsEnum)
                Cache.AttachEnumCodes(item);

            var routes = GetRoutes(item);
            PluginConfig.AddPlugin(item, routes);
            AddAppMenu(item, routes);
            AddMenu(item, routes);
            AddCodeInfo(item);
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
            if (values.Length > 1)
                info.Name = values[1].Trim();
            if (values.Length > 2)
                info.Icon = values[2].Trim();
            if (values.Length > 3)
                info.Style = values[3].Trim();
            if (values.Length > 4)
                info.Position = values[4].Trim();
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

    private static IEnumerable<RouteAttribute> GetRoutes(Type item)
    {
        var routes = item.GetCustomAttributes<RouteAttribute>();
        if (routes != null && routes.Any())
        {
            foreach (var route in routes)
            {
                Config.RouteTypes[route.Template] = item;
            }
        }
        return routes;
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
        if (menu != null)
        {
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
    }

    private static void AddMenu(Type item, IEnumerable<RouteAttribute> routes)
    {
        var menu = item.GetCustomAttribute<MenuAttribute>();
        if (menu != null)
        {
            menu.Page = item;
            menu.Url = routes?.FirstOrDefault()?.Template;
            Config.Menus.Add(menu);
        }
    }

    private static void AddFieldType(Type item)
    {
        if (item.Name == nameof(ICustomField) || item.Name == nameof(CustomField))
            return;

        Config.FieldTypes[item.Name] = item;
    }

    private static void AddCodeInfo(Type item)
    {
        var codeInfo = item.GetCustomAttribute<CodeInfoAttribute>();
        if (codeInfo != null)
            Cache.AttachCodes(item);
    }
}