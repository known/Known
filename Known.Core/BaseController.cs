namespace Known.Core;

public class BaseController : ControllerBase, IService
{
    private Context context;
    public Context Context
    {
        get { return GetContext(); }
        set { context = value; }
    }

    private Context GetContext()
    {
        if (context == null)
        {
            var token = HttpContext.Request.Headers[Constants.KeyToken].ToString();
            context = Context.Create(token);
        }

        return context;
    }
}