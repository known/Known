using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Known.Extensions;

namespace Known.Tests.KnownTests.Extensions
{
    public class CollectionExtensionTest
    {
        public static void TestNameValueCollectionToDictionary()
        {
            var nvc = new NameValueCollection
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };
            var dic = nvc.ToDictionary();
            Assert.IsEqual(dic["key1"], "value1");
            Assert.IsEqual(dic["key2"], "value2");
        }

        public static void TestIDictionaryValue()
        {
            var dic = new Dictionary<string, object>();
            dic["Key1"] = "test";
            dic["Key2"] = 1;
            dic["Key3"] = new TestEntity
            {
                Item1 = 1,
                Item2 = "test"
            };
            Assert.IsEqual(dic.Value<string>("Key1"), "test");
            Assert.IsEqual(dic.Value<int>("Key2"), 1);
            var key3 = dic.Value<TestEntity>("Key3");
            Assert.IsEqual(key3.Item2, "test");
        }
    }
}
