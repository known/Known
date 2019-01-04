using Known.Data;
using Known.Extensions;

namespace Known.Tests.Core.Data
{
    public class ColumnInfoTest
    {
        public static void TestGetColumnName()
        {
            var properties = typeof(TestEntity).GetColumnProperties();
            foreach (var item in properties)
            {
                var column = new ColumnInfo(item);
                if (item.Name == "Item1")
                    Assert.AreEqual(column.ColumnName, "item1");
                else if (item.Name == "Item4")
                    Assert.AreEqual(column.ColumnName, "item4");
                else if (item.Name == "Test")
                    Assert.AreEqual(column.ColumnName, "test_id");
            }
        }
    }
}
