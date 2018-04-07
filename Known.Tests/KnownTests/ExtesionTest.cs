using Known.Extensions;
using Known.Mapping;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;

namespace Known.Tests.KnownTests
{
    public class ExtesionTest
    {
        #region CollectionExtension
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
        #endregion

        #region CompressExtension
        public static void TestCompress()
        {
            var length1 = "test".ToBytes().Length;
            var length2 = "test".Compress().Length;
            Assert.IsEqual(length1 > length2, true);

            var set = new DataSet();
            set.Tables.Add(new DataTable());
            Assert.IsEqual(set.Compress().Length, 242);
        }

        public static void TestDecompress()
        {
            var bytes1 = "test".Compress();
            Assert.IsEqual(bytes1.Decompress<string>(), "test");

            var set = new DataSet();
            set.Tables.Add(new DataTable());
            var bytes = set.Compress();
            Assert.IsEqual(bytes.Decompress().Tables.Count, 1);
        }
        #endregion

        #region DateTimeExtension
        public static void TestDateTimeToTimestamp()
        {
            var date = new DateTime(2017, 10, 1, 10, 1, 1);
            Assert.IsEqual(date.ToTimestamp(), 1506823261000);
        }

        public static void TestStringToDateTime()
        {
            Assert.IsEqual("2017-10-01".ToDateTime("yyyy-MM-dd"), new DateTime(2017, 10, 1));
        }

        public static void TestDateTimeToString()
        {
            var date = new DateTime(2017, 10, 1, 10, 1, 1);
            Assert.IsEqual(date.ToString("yyyy-MM-dd HH:mm:ss"), "2017-10-01 10:01:01");
        }
        #endregion

        #region EnumExtension
        public static void TestEnumGetDescription()
        {
            Assert.IsEqual(TestEnum.Item1.GetDescription(), "枚举1");
            Assert.IsEqual(TestEnum.Item2.GetDescription(), "枚举2");
        }
        #endregion

        #region SerializeExtension
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
        #endregion

        #region StringExtension
        public static void TestAppendLine()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{0}不能为空！", "Item1");
            Assert.IsEqual(sb.ToString(), "Item1不能为空！" + Environment.NewLine);
        }

        public static void TestByteLength()
        {
            var value = "Item1不能为空！";
            Assert.IsEqual(value.ByteLength(), 15);
        }

        public static void TestByteSubstring()
        {
            var value = "Item1不能为空！";
            Assert.IsEqual(value.ByteSubstring(5), "不能为空！");
        }

        public static void TestByteSubstringWithLength()
        {
            var value = "Item1不能为空！";
            Assert.IsEqual(value.ByteSubstring(5, 4), "不能");
        }

        public static void TestFormatXml()
        {
            var value = new TestEntity { Item1 = 1, Item2 = "test", Item3 = new DateTime(2017, 10, 1) };
            var value1 = new TestEntity { Item1 = 1, Item2 = "test", Item3 = new DateTime(2017, 10, 1) };
            var valueXml = value.ToXml().Replace("  ", "").Replace(Environment.NewLine, "");
            Assert.IsEqual(valueXml.FormatXml(), value1.ToXml());
        }
        #endregion

        #region TypeExtension
        public static void TestGetColumnProperties()
        {
            var properties = typeof(TestEntity).GetColumnProperties();
            Assert.IsEqual(properties.Count, 8);
        }

        public static void TestGetAttribute()
        {
            var properties = typeof(TestEntity).GetColumnProperties();
            var item1 = properties.FirstOrDefault(p => p.Name == "Item1");
            var item3 = properties.FirstOrDefault(p => p.Name == "Item3");
            var attr1 = item1.GetAttribute<IntegerColumnAttribute>();
            var attr3 = item3.GetAttribute<DateTimeColumnAttribute>();
            Assert.IsEqual(attr1.ColumnName, "ITEM1");
            Assert.IsEqual(attr3.ColumnName, "ITEM3");
        }

        public static void TestHasColumnProperty()
        {
            Assert.IsEqual(typeof(TestEntity).HasColumnProperty("Item1"), true);
            Assert.IsEqual(typeof(TestEntity).HasColumnProperty("ItemOnlyRead"), false);
        }
        #endregion
    }
}
