using Known.Data;

namespace Known.Tests.KnownTests.Data
{
    public class CommandCacheTest
    {
        public static void TestGetCommand()
        {
            var sql = "select * from t_test where category=@category and code=@code";
            var command = CommandCache.GetCommand(sql, new { category = "test", code = "1" });
            Assert.IsEqual(command.Text, sql);
            Assert.IsEqual(command.Parameters.Count, 2);
            Assert.IsEqual(command.Parameters["category"], "test");
        }

        public static void TestGetSaveCommand()
        {
            var entity = new TestEntity { Item1 = 1, Item2 = "test" };
            var command = CommandCache.GetSaveCommand(entity);
            Assert.IsEqual(command.Text, "insert into A_TEST(ITEM1,ITEM2,ITEM3,Item4,TestId,Id,CreateBy,CreateTime,ModifyBy,ModifyTime) values(@ITEM1,@ITEM2,@ITEM3,@Item4,@TestId,@Id,@CreateBy,@CreateTime,@ModifyBy,@ModifyTime)");
            Assert.IsEqual(command.Parameters.Count, 10);
        }

        public static void TestGetDeleteCommand()
        {
            var entity1 = new TestEntity { Item1 = 1, Item2 = "test" };
            var command1 = CommandCache.GetDeleteCommand(entity1);
            Assert.IsEqual(command1.Text, "delete from A_TEST where ITEM1=@ITEM1 and ITEM2=@ITEM2");
            Assert.IsEqual(command1.Parameters.Count, 2);

            var entity2 = new TestObject { Item1 = "1", Item2 = "name" };
            var command2 = CommandCache.GetDeleteCommand(entity2);
            Assert.IsEqual(command2.Text, "delete from TestObjects where Id=@Id");
            Assert.IsEqual(command2.Parameters.Count, 1);
        }
    }
}
