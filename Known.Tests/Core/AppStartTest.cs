using Known.Core;

namespace Known.Tests.Core
{
    public class AppStartTest
    {
        public static void Start()
        {
            var context = Context.Create();
            Initializer.Initialize(context);
        }
    }
}
