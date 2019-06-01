using System.Linq;
using Known.Extensions;
using Known.Mapping;

namespace Known.Tests.Extensions
{
    public class TypeExtensionTest
    {
        public static void GetAttribute()
        {
            var properties = typeof(TestEntity).GetTypeProperties();
            var item1 = properties.FirstOrDefault(p => p.Name == "Item1");
            var item3 = properties.FirstOrDefault(p => p.Name == "Item3");
            var attr1 = item1.GetAttribute<IntegerColumnAttribute>();
            var attr3 = item3.GetAttribute<DateTimeColumnAttribute>();
            TestAssert.AreEqual(attr1.ColumnName, "item1");
            TestAssert.AreEqual(attr3.ColumnName, "item3");
        }
    }
}
