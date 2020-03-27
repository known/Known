using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Known.Extensions;

namespace Known.Tests.Extensions
{
    public class CollectionExtensionTest
    {
        public static void NameValueCollectionToDictionary()
        {
            var nvc = new NameValueCollection
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };
            var dic = nvc.ToDictionary();
            TestAssert.AreEqual(dic["key1"], "value1");
            TestAssert.AreEqual(dic["key2"], "value2");
        }

        public static void IDictionaryValue()
        {
            var dic = new Dictionary<string, object>();
            dic["Key1"] = "test";
            dic["Key2"] = 1;
            dic["Key3"] = new TestEntity
            {
                Item1 = 1,
                Item2 = "test"
            };
            TestAssert.AreEqual(dic.Value<string>("Key1"), "test");
            TestAssert.AreEqual(dic.Value<int>("Key2"), 1);
            var key3 = dic.Value<TestEntity>("Key3");
            TestAssert.AreEqual(key3.Item2, "test");
        }
    }
}
