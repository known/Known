using Known.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Known.Tests.KnownTests
{
    public class DataTest
    {
        public static void TestCommandCacheGetCommand()
        {
            var sql = "select * from t_test where category=@category and code=@code";
            var command = CommandCache.GetCommand(sql, new { category = "test", code = "1" });
            Assert.IsEqual(command.Text, sql);
            Assert.IsEqual(command.Parameters.Count, 2);
            Assert.IsEqual(command.Parameters["category"], "test");
        }

        public static void TestCommandCacheGetSaveCommand()
        {
            var entity = new TestEntity { Item1 = 1, Item2 = "test" };
            var command = CommandCache.GetSaveCommand(entity);
            Assert.IsEqual(command.Text, "insert into A_TEST(ITEM1,ITEM2,ITEM3) values(@ITEM1,@ITEM2,@ITEM3)");
            Assert.IsEqual(command.Parameters.Count, 3);
        }

        public static void TestCommandCacheGetDeleteCommand()
        {
            var entity = new TestEntity { Item1 = 1, Item2 = "test" };
            var command = CommandCache.GetDeleteCommand(entity);
            Assert.IsEqual(command.Text, "delete from A_TEST where ITEM1=@ITEM1");
            Assert.IsEqual(command.Parameters.Count, 1);
        }
    }
}
