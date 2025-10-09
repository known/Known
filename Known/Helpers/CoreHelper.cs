namespace Known.Helpers;

static class CoreHelper
{
    internal static void LoadType(this IServiceCollection services, XmlDocument doc, Type type, object[] attributes)
    {
        var routes = attributes.OfType<RouteAttribute>().ToList();
        var role = attributes.OfType<RoleAttribute>().FirstOrDefault();
        var service = attributes.OfType<ServiceAttribute>().FirstOrDefault();
        var webApi = attributes.OfType<WebApiAttribute>().FirstOrDefault();
        var task = attributes.OfType<TaskAttribute>().FirstOrDefault();
        var import = attributes.OfType<ImportAttribute>().FirstOrDefault();

        if (role != null || (routes != null && routes.Count > 0))
            MenuHelper.AddMenu(type, role, routes, attributes);
        else if (service != null)
            services.AddServices(service.Lifetime, type);
        //else if (type.IsInterface && !type.IsGenericTypeDefinition && type.IsAssignableTo(typeof(IService)) && type.Name != nameof(IService))
        //    AddApiMethod(doc, type, type.Name[1..].Replace("Service", ""));
        //else if (TypeHelper.IsGenericSubclass(type, typeof(EntityTablePage<>), out var arguments))
        //    AddApiMethod(doc, typeof(IEntityService<>).MakeGenericType(arguments), type.Name);
        else if (type.IsAssignableTo(typeof(EntityBase)) && type.Name != nameof(EntityBase))
            DbConfig.Models.Add(type);
        else if (task != null)
            services.AddTask(type, task);
        else if (import != null || (type.IsAssignableTo(typeof(ImportBase)) && type.Name != nameof(ImportBase)))
            services.AddImport(type, import);
        else if (type.IsAssignableTo(typeof(FlowBase)) && type.Name != nameof(FlowBase))
            services.AddFlow(type);

        if (webApi != null)
            AddApiMethod(doc, type, type.Name.Replace("Service", ""));
    }

    private static void AddApiMethod(XmlDocument doc, Type type, string apiName)
    {
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
                CoreConfig.ApiMethods.Add(info);
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

    private static void AddTask(this IServiceCollection services, Type type, TaskAttribute task)
    {
        CoreConfig.TaskTypes[task.BizType] = type;
        services.AddScoped(type);
    }

    private static void AddImport(this IServiceCollection services, Type type, ImportAttribute import)
    {
        var key = import != null ? import.Type.Name : type.Name.Replace("Import", "");
        CoreConfig.ImportTypes[key] = type;
        services.AddScoped(type);
    }

    private static void AddFlow(this IServiceCollection services, Type type)
    {
        CoreConfig.FlowTypes[type.Name] = type;
        services.AddScoped(type);
    }
}