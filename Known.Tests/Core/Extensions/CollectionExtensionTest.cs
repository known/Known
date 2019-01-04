using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Known.Extensions;

namespace Known.Tests.Core.Extensions
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
            Assert.AreEqual(dic["key1"], "value1");
            Assert.AreEqual(dic["key2"], "value2");
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
            Assert.AreEqual(dic.Value<string>("Key1"), "test");
            Assert.AreEqual(dic.Value<int>("Key2"), 1);
            var key3 = dic.Value<TestEntity>("Key3");
            Assert.AreEqual(key3.Item2, "test");
        }
    }
}
