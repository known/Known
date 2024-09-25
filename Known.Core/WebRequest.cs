namespace Known.Core;

class WebRequest(HttpContext context) : IRequest
{
    private readonly HttpContext Context = context;
    private HttpRequest Request => Context.Request;
    private bool IsPost => Request.Method == "POST" && Request.Form != null;

    public bool IsHandler(string name)
    {
        if (!IsPost)
            return false;

        if (!Request.Form.ContainsKey("_handler"))
            return false;

        var handler = Request.Form["_handler"];
        return handler == name;
    }

    public T Get<T>(string name)
    {
        var value = string.Empty;
        if (Request.Query.ContainsKey(name))
            value = Request.Query[name];
        else if (IsPost && Request.Form.ContainsKey(name))
            value = Request.Form[name];

        return Utils.ConvertTo<T>(value);
    }

    public T GetModel<T>()
    {
        if (!IsPost)
            return default;

        var model = Activator.CreateInstance<T>();
        foreach (var item in Request.Form)
        {
            if (item.Key.StartsWith('_'))
                continue;

            TypeHelper.SetPropertyValue(model, item.Key, item.Value);
        }
        return model;
    }
}