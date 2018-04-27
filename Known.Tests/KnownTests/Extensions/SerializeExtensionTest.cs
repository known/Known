using System;
using Known.Extensions;

namespace Known.Tests.KnownTests.Extensions
{
    public class SerializeExtensionTest
    {
        public static void TestToJson()
        {
            var value = new TestEntity { Item1 = 1, Item2 = "test", Item3 = new DateTime(2017, 10, 1) };
            var value1 = new TestEntity { Item1 = 1, Item2 = "test", Item3 = new DateTime(2017, 10, 1) };
            Assert.IsEqual(value.ToJson(), value1.ToJson());
        }

        public static void TestFromJson()
        {
            var value = new TestEntity { Item1 = 1, Item2 = "test", Item3 = new DateTime(2017, 10, 1) };
            var json = value.ToJson();
            var value1 = json.FromJson<TestEntity>();
            Assert.IsEqual(value1.Item1, value.Item1);
            Assert.IsEqual(value1.Item2, value.Item2);
            Assert.IsEqual(value1.Item3, value.Item3);
        }

        public static void TestToXml()
        {
            var value = new TestEntity { Item1 = 1, Item2 = "test", Item3 = new DateTime(2017, 10, 1) };
            var value1 = new TestEntity { Item1 = 1, Item2 = "test", Item3 = new DateTime(2017, 10, 1) };
            Assert.IsEqual(value.ToXml(), value1.ToXml());
        }

        public static void TestFromXml()
        {
            var value = new TestEntity { Item1 = 1, Item2 = "test", Item3 = new DateTime(2017, 10, 1) };
            var json = value.ToXml();
            var value1 = json.FromXml<TestEntity>();
            Assert.IsEqual(value1.Item1, value.Item1);
            Assert.IsEqual(value1.Item2, value.Item2);
            Assert.IsEqual(value1.Item3, value.Item3);
        }

        public static void TestToBytes()
        {
            var value = "test";
            var value1 = "test";
            Assert.IsEqual(value.ToBytes().Length, value1.ToBytes().Length);
        }

        public static void TestFromBytes()
        {
            var value = "test";
            var bytes = value.ToBytes();
            Assert.IsEqual(bytes.FromBytes(), value);
        }
    }
}
