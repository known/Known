using System.Collections.Generic;
using Known.Data;
using Known.Extensions;

namespace Known.Tests.Core.Data
{
    public class CommandTest
    {
        public static void TestText()
        {
            var text = "test";
            var command = new Command(text);
            TestAssert.AreEqual(command.Text, text);
        }

        public static void TestParameters()
        {
            var command = new Command("test");
            TestAssert.IsNotNull(command.Parameters);

            var parameters = new Dictionary<string, object>();
            parameters.Add("item1", 1);
            parameters.Add("item2", 2);
            command = new Command("test", parameters);
            TestAssert.AreEqual(command.Parameters.Count, parameters.Count);
        }

        public static void TestHasParameter()
        {
            var command = new Command("test");
            TestAssert.AreEqual(command.HasParameter, false);

            var parameters = new Dictionary<string, object>();
            parameters.Add("item1", 1);
            parameters.Add("item2", 2);
            command = new Command("test", parameters);
            TestAssert.AreEqual(command.HasParameter, true);
        }

        public static void TestAddParameter()
        {
            var command = new Command("test");
            command.AddParameter("item1", 1);
            command.AddParameter("item2", 2);
            TestAssert.AreEqual(command.Parameters["item1"], 1);
            TestAssert.AreEqual(command.Parameters["item2"], 2);

            command.AddParameter("item1", 3);
            TestAssert.AreEqual(command.Parameters["item1"], 3);
        }

        public static void TestToString()
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("item1", 1);
            parameters.Add("item2", 2);
            var command = new Command("test", parameters);
            TestAssert.AreEqual(command.ToString(), $"Text=test\r\nParameters={parameters.ToJson()}\r\n");
        }
    }
}
