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
                var columnName = ColumnInfo.GetColumnName(item);
                if (item.Name == "Item1")
                    Assert.IsEqual(columnName, "item1");
                else if (item.Name == "Item4")
                    Assert.IsEqual(columnName, "item4");
                else if (item.Name == "Test")
                    Assert.IsEqual(columnName, "test_id");
            }
        }
    }
}
