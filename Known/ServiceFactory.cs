namespace Known;

public sealed class ServiceFactory
{
    private ServiceFactory() { }

    public static T Create<T>(Context context) where T : IService
    {
        var key = typeof(T).Name[1..];
        if (!Config.ServiceTypes.TryGetValue(key, out Type type))
            throw new SystemException($"The service type {key} is not register!");

        var service = Activator.CreateInstance(type, context);
        if (service == null)
            throw new SystemException($"The service type {key} is not implement!");

        return (T)service;
    }
}