namespace Known.Core
{
    public sealed class Initializer
    {
        public static void Initialize(Context context = null)
        {
            if (context == null)
            {
                context = Context.Create();
            }

            var assembly = typeof(Initializer).Assembly;
            Container.Register<ServiceBase>(assembly, context);
        }
    }
}
