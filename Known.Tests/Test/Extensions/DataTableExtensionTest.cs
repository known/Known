using System;
using System.Data;
using Known.Extensions;

namespace Known.Tests.Extensions
{
    public class DataTableExtensionTest
    {
        public static void ToPageTable()
        {
            var table = new DataTable();
            table.Columns.Add("Column1");
            table.Columns.Add("Column2");
            table.Columns.Add("Column3");

            for (int i = 0; i < 25; i++)
            {
                table.Rows.Add(i, $"Code{i}", $"Name{i}");
            }

            var page = table.ToPageTable(1, 10);
            TestAssert.AreEqual(page.Rows.Count, 10);
            TestAssert.AreEqual(page.Rows[0][0], "10");
            TestAssert.AreEqual(page.Rows[9][1], "Code19");
        }

        public static void ColumnSameAs()
        {
            //Original
            var table = new DataTable();
            table.Columns.Add("Column1");
            table.Columns.Add("Column2");
            table.Columns.Add("Column3");

            //True
            var table1 = new DataTable();
            table1.Columns.Add("Column1");
            table1.Columns.Add("Column2");
            table1.Columns.Add("Column3");

            var error = string.Empty;
            var result = table1.ColumnSameAs(table, out error);
            TestAssert.IsTrue(result);

            //False
            var table2 = new DataTable();
            table2.Columns.Add("Column1");
            table2.Columns.Add("Column2");

            result = table2.ColumnSameAs(table, out error);
            TestAssert.IsFalse(result);
            TestAssert.AreEqual(error, "栏位条数不一致！");

            //False
            var table3 = new DataTable();
            table3.Columns.Add("Column1");
            table3.Columns.Add("Column22");
            table3.Columns.Add("Column33");

            result = table3.ColumnSameAs(table, out error);
            TestAssert.IsFalse(result);
            TestAssert.AreEqual(error, "不存在【Column22,Column33】这些栏位！");
        }

        public static void Get()
        {
            var table = new DataTable();
            table.Columns.Add("Column1", typeof(int));
            table.Columns.Add("Column2", typeof(string));
            table.Columns.Add("Column3", typeof(DateTime));

            var column1 = 1;
            var column2 = "Code";
            var column3 = new DateTime(2018, 1, 1);
            table.Rows.Add(column1, column2, new DateTime(2018, 1, 1));

            TestAssert.AreEqual(table.Rows[0].Get<int>("Column1"), column1);
            TestAssert.AreEqual(table.Rows[0].Get<string>("Column2"), column2);
            TestAssert.AreEqual(table.Rows[0].Get<DateTime>("Column3"), column3);
        }
    }
}
