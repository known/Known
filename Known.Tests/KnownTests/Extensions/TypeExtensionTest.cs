using System.Linq;
using Known.Extensions;
using Known.Mapping;

namespace Known.Tests.KnownTests.Extensions
{
    public class TypeExtensionTest
    {
        public static void TestGetColumnProperties()
        {
            var properties = typeof(TestEntity).GetColumnProperties();
            Assert.IsEqual(properties.Count, 11);
        }

        public static void TestGetAttribute()
        {
            var properties = typeof(TestEntity).GetColumnProperties();
            var item1 = properties.FirstOrDefault(p => p.Name == "Item1");
            var item3 = properties.FirstOrDefault(p => p.Name == "Item3");
            var attr1 = item1.GetAttribute<IntegerColumnAttribute>();
            var attr3 = item3.GetAttribute<DateTimeColumnAttribute>();
            Assert.IsEqual(attr1.ColumnName, "item1");
            Assert.IsEqual(attr3.ColumnName, "item3");
        }

        public static void TestHasColumnProperty()
        {
            Assert.IsEqual(typeof(TestEntity).HasColumnProperty("Item1"), true);
            Assert.IsEqual(typeof(TestEntity).HasColumnProperty("ItemOnlyRead"), false);
        }
    }
}
