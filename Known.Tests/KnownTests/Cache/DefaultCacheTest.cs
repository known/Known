using Known.Cache;

namespace Known.Tests.KnownTests.Cache
{
    public class DefaultCacheTest
    {
        private static ICache cache = new DefaultCache();

        public static void TestGet()
        {
            cache.Set("Key", "Value");
            Assert.IsNull(cache.Get(""));
            Assert.IsNull(cache.Get("test"));
            Assert.IsEqual(cache.Get("Key"), "Value");
        }

        public static void TestSet()
        {
            cache.Set("Key", null);
            Assert.IsNull(cache.Get("Key"));
            cache.Set("Key", "Value");
            Assert.IsEqual(cache.Get("Key"), "Value");
        }

        public static void TestRemove()
        {
            cache.Set("Key", "Value");
            Assert.IsEqual(cache.Get("Key"), "Value");
            cache.Remove("Key");
            Assert.IsNull(cache.Get("Key"));
        }

        public static void TestRemoveAll()
        {
            cache.Set("Key1", "Value1");
            cache.Set("Key2", "Value2");
            cache.RemoveAll();
            Assert.IsNull(cache.Get("Key1"));
            Assert.IsNull(cache.Get("Key2"));
        }
    }
}
