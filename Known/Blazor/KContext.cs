namespace Known.Blazor;

public class KContext : CascadingValue<Context>
{
    public KContext()
    {
        IsFixed = true;
    }
}