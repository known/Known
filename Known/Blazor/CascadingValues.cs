namespace Known.Blazor;

public class KContext : CascadingValue<Context>
{
    public KContext()
    {
        IsFixed = true;
    }
}

public class KRouteData : CascadingValue<RouteData>
{
    public KRouteData()
    {
        IsFixed = true;
    }
}