using System.Collections.Generic;
using System.Data;
using Known.Data;

namespace Known.Tests.KnownTests.Data
{
    public class CommandCacheTest
    {
        public static void TestGetCommand()
        {
            var command = CommandCache.GetCommand(null);
            Assert.IsNull(command);

            command = CommandCache.GetCommand("");
            Assert.IsNull(command);

            var sql = "select * from t_test";
            command = CommandCache.GetCommand(sql);
            Assert.IsEqual(command.Text, sql);

            sql = "select * from t_test where category=@category and code=@code";
            command = CommandCache.GetCommand(sql, new { category = "test", code = "1" });
            Assert.IsEqual(command.Text, sql);
            Assert.IsEqual(command.Parameters.Count, 2);
            Assert.IsEqual(command.Parameters["category"], "test");
            Assert.IsEqual(command.Parameters["code"], "1");
        }

        public static void TestGetSaveCommand()
        {
            TestEntity entity = null;
            var command = CommandCache.GetSaveCommand(entity);
            Assert.IsNull(command);

            entity = new TestEntity { Item1 = 1, Item2 = "test" };
            command = CommandCache.GetSaveCommand(entity);
            Assert.IsEqual(command.Text, "insert into A_TEST(ITEM1,ITEM2,ITEM3,Item4,TestId,Id,CreateBy,CreateTime,ModifyBy,ModifyTime) values(@ITEM1,@ITEM2,@ITEM3,@Item4,@TestId,@Id,@CreateBy,@CreateTime,@ModifyBy,@ModifyTime)");
            Assert.IsEqual(command.Parameters.Count, 10);
            Assert.IsEqual(command.Parameters["ITEM1"], 1);
            Assert.IsEqual(command.Parameters["ITEM2"], "test");
        }

        public static void TestGetDeleteCommand()
        {
            TestEntity entity = null;
            var command = CommandCache.GetDeleteCommand(entity);
            Assert.IsNull(command);

            entity = new TestEntity { Item1 = 1, Item2 = "test" };
            command = CommandCache.GetDeleteCommand(entity);
            Assert.IsEqual(command.Text, "delete from A_TEST where ITEM1=@ITEM1 and ITEM2=@ITEM2");
            Assert.IsEqual(command.Parameters.Count, 2);
            Assert.IsEqual(command.Parameters["ITEM1"], 1);
            Assert.IsEqual(command.Parameters["ITEM2"], "test");

            var obj = new TestObject { Id = "1", Item1 = "test", Item2 = "name" };
            command = CommandCache.GetDeleteCommand(obj);
            Assert.IsEqual(command.Text, "delete from TestObjects where Id=@Id");
            Assert.IsEqual(command.Parameters.Count, 1);
            Assert.IsEqual(command.Parameters["Id"], "1");

            command = CommandCache.GetDeleteCommand(null, null);
            Assert.IsNull(command);

            command = CommandCache.GetDeleteCommand("", null);
            Assert.IsNull(command);

            command = CommandCache.GetDeleteCommand("t_test", null);
            Assert.IsNull(command);

            var parameters = new Dictionary<string, object>();
            parameters.Add("column1", 1);
            parameters.Add("column2", 2);
            command = CommandCache.GetDeleteCommand("t_test", parameters);
            Assert.IsEqual(command.Text, "delete from t_test where column1=@column1 and column2=@column2");
            Assert.IsEqual(command.Parameters.Count, parameters.Count);
        }

        public static void TestGetSelectCommand()
        {
            var command = CommandCache.GetSelectCommand(null);
            Assert.IsNull(command);

            command = CommandCache.GetSelectCommand("");
            Assert.IsNull(command);

            command = CommandCache.GetSelectCommand("t_test");
            Assert.IsEqual(command.Text, "select * from t_test");
            Assert.IsEqual(command.Parameters.Count, 0);

            var parameters = new Dictionary<string, object>();
            parameters.Add("Item1", 1);
            parameters.Add("Item2", 2);
            command = CommandCache.GetSelectCommand("t_test", parameters);
            Assert.IsEqual(command.Text, "select * from t_test where Item1=@Item1 and Item2=@Item2");
            Assert.IsEqual(command.Parameters.Count, parameters.Count);
            Assert.IsEqual(command.Parameters["Item1"], 1);
            Assert.IsEqual(command.Parameters["Item2"], 2);
        }

        public static void TestGetInsertCommand()
        {
            var command = CommandCache.GetInsertCommand(null);
            Assert.IsNull(command);

            var table = new DataTable();
            command = CommandCache.GetInsertCommand(table);
            Assert.IsNull(command);

            table.TableName = "t_test";
            table.Columns.Add("column1");
            table.Columns.Add("column2");
            command = CommandCache.GetInsertCommand(table);
            Assert.IsEqual(command.Text, "insert into t_test(column1,column2) values(@column1,@column2)");
            Assert.IsEqual(command.Parameters.Count, 0);

            command = CommandCache.GetInsertCommand("t_test", null);
            Assert.IsNull(command);

            var parameters = new Dictionary<string, object>();
            parameters.Add("column1", 1);
            parameters.Add("column2", 2);
            command = CommandCache.GetInsertCommand("t_test", parameters);
            Assert.IsEqual(command.Text, "insert into t_test(column1,column2) values(@column1,@column2)");
            Assert.IsEqual(command.Parameters.Count, parameters.Count);
        }

        public static void TestGetUpdateCommand()
        {
            var command = CommandCache.GetUpdateCommand(null, null, null);
            Assert.IsNull(command);

            command = CommandCache.GetUpdateCommand("", null, null);
            Assert.IsNull(command);

            command = CommandCache.GetUpdateCommand("t_test", null, null);
            Assert.IsNull(command);

            command = CommandCache.GetUpdateCommand("t_test", "", null);
            Assert.IsNull(command);

            command = CommandCache.GetUpdateCommand("t_test", "key1,key2", null);
            Assert.IsNull(command);

            var parameters = new Dictionary<string, object>();
            parameters.Add("key1", 1);
            parameters.Add("key2", 2);
            parameters.Add("column1", 1);
            parameters.Add("column2", 2);
            command = CommandCache.GetUpdateCommand("t_test", "key1,key2", parameters);
            Assert.IsEqual(command.Text, "update t_test set column1=@column1,column2=@column2 where key1=@key1 and key2=@key2");
            Assert.IsEqual(command.Parameters.Count, parameters.Count);
        }
    }
}
