using Known.Mapping;

namespace Known.Core
{
    public sealed class Initializer
    {
        public static void Initialize()
        {
            var assembly = typeof(Initializer).Assembly;
            EntityHelper.InitMapper(assembly);
        }
    }
}
