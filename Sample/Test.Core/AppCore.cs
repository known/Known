namespace Test.Core;

public class AppCore
{
    public static void Initialize()
    {
        var assembly = typeof(AppCore).Assembly;
        BaseImport.Register(assembly);
        BaseFlow.Register(assembly);
    }
}