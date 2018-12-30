using Known.Data;
using Known.Extensions;

namespace Known.Tests.KnownTests.Data
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
                    Assert.IsEqual(column.ColumnName, "item1");
                else if (item.Name == "Item4")
                    Assert.IsEqual(column.ColumnName, "item4");
                else if (item.Name == "Test")
                    Assert.IsEqual(column.ColumnName, "test_id");
            }
        }
    }
}
