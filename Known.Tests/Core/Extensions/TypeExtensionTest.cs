using System.Linq;
using Known.Extensions;
using Known.Mapping;

namespace Known.Tests.Core.Extensions
{
    public class TypeExtensionTest
    {
        public static void TestGetColumnProperties()
        {
            var properties = typeof(TestEntity).GetColumnProperties();
            Assert.AreEqual(properties.Count, 11);
        }

        public static void TestGetAttribute()
        {
            var properties = typeof(TestEntity).GetColumnProperties();
            var item1 = properties.FirstOrDefault(p => p.Name == "Item1");
            var item3 = properties.FirstOrDefault(p => p.Name == "Item3");
            var attr1 = item1.GetAttribute<IntegerColumnAttribute>();
            var attr3 = item3.GetAttribute<DateTimeColumnAttribute>();
            Assert.AreEqual(attr1.ColumnName, "item1");
            Assert.AreEqual(attr3.ColumnName, "item3");
        }

        public static void TestHasColumnProperty()
        {
            Assert.AreEqual(typeof(TestEntity).HasColumnProperty("Item1"), true);
            Assert.AreEqual(typeof(TestEntity).HasColumnProperty("ItemOnlyRead"), false);
        }
    }
}
