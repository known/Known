namespace Known.Platform
{
    public sealed class Initializer
    {
        public static void Initialize(Context context)
        {
            var assembly = typeof(Initializer).Assembly;
            Container.Register<ServiceBase>(assembly, context);
        }
    }
}
