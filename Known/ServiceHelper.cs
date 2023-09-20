namespace Known;

class ServiceHelper
{
    internal static Task<string> GetAsync(Context context, string url)
    {
        var result = Invoke(context, url);
        if (result == null)
            return Task.FromResult("");

        return Task.FromResult(result.ToString());
    }

    internal static Task<TResult> GetAsync<TResult>(Context context, string url)
    {
        var result = Invoke(context, url);
        if (result == null)
            return Task.FromResult(default(TResult));

        return Task.FromResult((TResult)result);
    }

    internal static Task<Result> PostAsync(Context context, string url)
    {
        var result = Invoke(context, url);
        if (result == null)
            return Task.FromResult(Result.Error("操作失败！"));

        return Task.FromResult((Result)result);
    }

    internal static Task<TResult> PostAsync<TParam, TResult>(Context context, string url, TParam data)
    {
        var result = Invoke(context, url, data);
        if (result == null)
            return Task.FromResult(default(TResult));

        return Task.FromResult((TResult)result);
    }

    private static Tuple<string, string, Dictionary<string, string>> ParseUrl(string url)
    {
        var item = Tuple.Create("", "", new Dictionary<string, string>());
        if (string.IsNullOrWhiteSpace(url))
            return item;

        var urls = url.Split('/', '?', '&');
        if (urls.Length < 2)
            return item;

        var parms = new Dictionary<string, string>();
        foreach (var param in urls)
        {
            if (param.Contains('='))
            {
                var values = param.Split('=');
                parms[values[0]] = values[1];
            }
        }

        return Tuple.Create(urls[0], urls[1], parms);
    }

    private static object Invoke(Context context, string url, object data = null)
    {
        var item = ParseUrl(url);
        var serviceName = $"{item.Item1}Service";
        if (!Container.RegTypes.ContainsKey(serviceName))
            throw new Exception($"不存在{serviceName}服务！");

        var type = Container.RegTypes[serviceName];
        var method = type.GetMethod(item.Item2, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (method == null)
            throw new Exception($"不存在{item.Item2}方法！");

        var service = CreateInstance(type, context) as ServiceBase;
        var paras = new List<object>();
        var parameters = method.GetParameters();
        if (parameters == null || parameters.Length == 0)
            return method.Invoke(service, null);

        if (item.Item3.Count > 0)
        {
            foreach (var parm in parameters)
            {
                var value = item.Item3.ContainsKey(parm.Name)
                          ? Utils.ConvertTo(parm.ParameterType, item.Item3[parm.Name], null)
                          : null;
                paras.Add(value);
            }
        }
        else
        {
            paras.Add(data);
        }
        return method.Invoke(service, paras.ToArray());
    }

    private static object CreateInstance(Type type, params object[] args)
    {
        var instance = type.Assembly.CreateInstance(
            type.FullName, false,
            BindingFlags.Instance | BindingFlags.NonPublic,
            null, args, null, null);
        return instance;
    }
}