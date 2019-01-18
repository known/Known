using Known.Extensions;
using Known.Mapping;

namespace Known.Tests.Core.Data
{
    public class ColumnInfoTest
    {
        public static void GetColumnName()
        {
            var properties = typeof(TestEntity).GetColumnProperties();
            foreach (var item in properties)
            {
                var column = new ColumnInfo(item);
                if (item.Name == "Item1")
                    TestAssert.AreEqual(column.ColumnName, "item1");
                else if (item.Name == "Item4")
                    TestAssert.AreEqual(column.ColumnName, "item4");
                else if (item.Name == "Test")
                    TestAssert.AreEqual(column.ColumnName, "test_id");
            }
        }
    }
}
