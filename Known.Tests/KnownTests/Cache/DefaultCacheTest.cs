using Known.Cache;

namespace Known.Tests.KnownTests.Cache
{
    public class DefaultCacheTest
    {
        private static ICache cache = new DefaultCache();

        public static void TestCount()
        {
            cache.RemoveAll();
            cache.Set("Key1", "Value1");
            Assert.IsEqual(cache.Count, 1);
            cache.Set("Key1", "Value2");
            Assert.IsEqual(cache.Count, 1);
            cache.Set("Key2", "Value2");
            Assert.IsEqual(cache.Count, 2);
        }

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
            cache.Set("Key", "Value1");
            Assert.IsEqual(cache.Get("Key"), "Value1");
        }

        public static void TestRemove()
        {
            cache.Set("Key", "Value");
            cache.Remove("Key");
            Assert.IsNull(cache.Get("Key"));
        }

        public static void TestRemoveAll()
        {
            cache.Set("Key1", "Value1");
            cache.Set("Key2", "Value2");
            Assert.IsEqual(cache.Count, 2);
            cache.RemoveAll();
            Assert.IsEqual(cache.Count, 0);
        }
    }
}
