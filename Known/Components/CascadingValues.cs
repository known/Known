﻿namespace Known.Components;

public class KContext : CascadingValue<UIContext>
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