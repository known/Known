using System.Collections.Generic;
using System.Data;
using Known.Data;

namespace Known.Tests.Core.Data
{
    public class CommandHelperTest
    {
        public static void TestGetCommand()
        {
            var command = CommandHelper.GetCommand(null);
            TestAssert.IsNull(command);

            command = CommandHelper.GetCommand("");
            TestAssert.IsNull(command);

            var sql = "select * from t_test";
            command = CommandHelper.GetCommand(sql);
            TestAssert.AreEqual(command.Text, sql);

            sql = "select * from t_test where category=@category and code=@code";
            command = CommandHelper.GetCommand(sql, new { category = "test", code = "1" });
            TestAssert.AreEqual(command.Text, sql);
            TestAssert.AreEqual(command.Parameters.Count, 2);
            TestAssert.AreEqual(command.Parameters["category"], "test");
            TestAssert.AreEqual(command.Parameters["code"], "1");
        }

        public static void TestGetSaveCommand()
        {
            TestEntity entity = null;
            var command = CommandHelper.GetSaveCommand(entity);
            TestAssert.IsNull(command);

            entity = new TestEntity { Item1 = 1, Item2 = "test" };
            command = CommandHelper.GetSaveCommand(entity);
            TestAssert.AreEqual(command.Text, "insert into a_test(id,create_by,create_time,modify_by,modify_time,extension,item1,item2,item3,item4,test_id) values(@id,@create_by,@create_time,@modify_by,@modify_time,@extension,@item1,@item2,@item3,@item4,@test_id)");
            TestAssert.AreEqual(command.Parameters.Count, 11);
            TestAssert.AreEqual(command.Parameters["item1"], 1);
            TestAssert.AreEqual(command.Parameters["item2"], "test");
        }

        public static void TestGetDeleteCommand()
        {
            TestEntity entity = null;
            var command = CommandHelper.GetDeleteCommand(entity);
            TestAssert.IsNull(command);

            entity = new TestEntity { Item1 = 1, Item2 = "test" };
            command = CommandHelper.GetDeleteCommand(entity);
            TestAssert.AreEqual(command.Text, "delete from a_test where item1=@item1 and item2=@item2");
            TestAssert.AreEqual(command.Parameters.Count, 2);
            TestAssert.AreEqual(command.Parameters["item1"], 1);
            TestAssert.AreEqual(command.Parameters["item2"], "test");

            var obj = new TestObject { Id = "1", Item1 = "test", Item2 = "name" };
            command = CommandHelper.GetDeleteCommand(obj);
            TestAssert.AreEqual(command.Text, "delete from testobjects where id=@id");
            TestAssert.AreEqual(command.Parameters.Count, 1);
            TestAssert.AreEqual(command.Parameters["id"], "1");

            command = CommandHelper.GetDeleteCommand(null, null);
            TestAssert.IsNull(command);

            command = CommandHelper.GetDeleteCommand("", null);
            TestAssert.IsNull(command);

            command = CommandHelper.GetDeleteCommand("t_test", null);
            TestAssert.IsNull(command);

            var parameters = new Dictionary<string, object>();
            parameters.Add("column1", 1);
            parameters.Add("column2", 2);
            command = CommandHelper.GetDeleteCommand("t_test", parameters);
            TestAssert.AreEqual(command.Text, "delete from t_test where column1=@column1 and column2=@column2");
            TestAssert.AreEqual(command.Parameters.Count, parameters.Count);
        }

        public static void TestGetSelectCommand()
        {
            var command = CommandHelper.GetSelectCommand(null);
            TestAssert.IsNull(command);

            command = CommandHelper.GetSelectCommand("");
            TestAssert.IsNull(command);

            command = CommandHelper.GetSelectCommand("t_test");
            TestAssert.AreEqual(command.Text, "select * from t_test");
            TestAssert.AreEqual(command.Parameters.Count, 0);

            var parameters = new Dictionary<string, object>();
            parameters.Add("item1", 1);
            parameters.Add("item2", 2);
            command = CommandHelper.GetSelectCommand("t_test", parameters);
            TestAssert.AreEqual(command.Text, "select * from t_test where item1=@item1 and item2=@item2");
            TestAssert.AreEqual(command.Parameters.Count, parameters.Count);
            TestAssert.AreEqual(command.Parameters["item1"], 1);
            TestAssert.AreEqual(command.Parameters["item2"], 2);
        }

        public static void TestGetInsertCommand()
        {
            var command = CommandHelper.GetInsertCommand(null);
            TestAssert.IsNull(command);

            var table = new DataTable();
            command = CommandHelper.GetInsertCommand(table);
            TestAssert.IsNull(command);

            table.TableName = "t_test";
            table.Columns.Add("column1");
            table.Columns.Add("column2");
            command = CommandHelper.GetInsertCommand(table);
            TestAssert.AreEqual(command.Text, "insert into t_test(column1,column2) values(@column1,@column2)");
            TestAssert.AreEqual(command.Parameters.Count, 0);

            command = CommandHelper.GetInsertCommand("t_test", null);
            TestAssert.IsNull(command);

            var parameters = new Dictionary<string, object>();
            parameters.Add("column1", 1);
            parameters.Add("column2", 2);
            command = CommandHelper.GetInsertCommand("t_test", parameters);
            TestAssert.AreEqual(command.Text, "insert into t_test(column1,column2) values(@column1,@column2)");
            TestAssert.AreEqual(command.Parameters.Count, parameters.Count);
        }

        public static void TestGetUpdateCommand()
        {
            var command = CommandHelper.GetUpdateCommand(null, null, null);
            TestAssert.IsNull(command);

            command = CommandHelper.GetUpdateCommand("", null, null);
            TestAssert.IsNull(command);

            command = CommandHelper.GetUpdateCommand("t_test", null, null);
            TestAssert.IsNull(command);

            command = CommandHelper.GetUpdateCommand("t_test", "", null);
            TestAssert.IsNull(command);

            command = CommandHelper.GetUpdateCommand("t_test", "key1,key2", null);
            TestAssert.IsNull(command);

            var parameters = new Dictionary<string, object>();
            parameters.Add("key1", 1);
            parameters.Add("key2", 2);
            parameters.Add("column1", 1);
            parameters.Add("column2", 2);
            command = CommandHelper.GetUpdateCommand("t_test", "key1,key2", parameters);
            TestAssert.AreEqual(command.Text, "update t_test set column1=@column1,column2=@column2 where key1=@key1 and key2=@key2");
            TestAssert.AreEqual(command.Parameters.Count, parameters.Count);
        }
    }
}
