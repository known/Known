using Known.Cache;

namespace Known.Tests.KnownTests.Cache
{
    public class DefaultCacheTest
    {
        public static void TestGet()
        {
            var cache = new DefaultCache();
            cache.Set("Key", "Value");
            Assert.IsNull(cache.Get(""));
            Assert.IsNull(cache.Get("test"));
            Assert.IsEqual(cache.Get("Key"), "Value");
        }
    }
}
