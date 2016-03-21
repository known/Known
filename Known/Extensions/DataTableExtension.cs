using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace System
{
    public static class DataTableExtension
    {
        public static bool ColumnSameAs(this DataTable table, DataTable compare)
        {
            if (table == null || compare == null)
                return false;

            if (table.Columns.Count != compare.Columns.Count)
                return false;

            var count = 0;
            foreach (DataColumn column in table.Columns)
            {
                if (compare.Columns.Contains(column.ColumnName))
                {
                    count += 1;
                }
            }

            return count == table.Columns.Count;
        }

        public static DataTable ToPageTable(this DataTable table, int pageIndex, int pageSize, ref int count)
        {
            if (table == null || table.Rows.Count == 0)
                return null;

            count = table.Rows.Count;
            return table.AsEnumerable().Skip((pageIndex - 1) * pageSize).Take(pageSize).CopyToDataTable();
        }

        public static void ForEach(this DataTable table, Action<DataRow> action)
        {
            if (table == null || table.Rows.Count == 0)
                return;

            foreach (DataRow row in table.Rows)
            {
                action(row);
            }
        }

        public static void ForEach(this DataTable table, Action<DataRow, int> action)
        {
            if (table == null || table.Rows.Count == 0)
                return;

            var i = 0;
            foreach (DataRow row in table.Rows)
            {
                action(row, i++);
            }
        }

        public static List<T> ToList<T>(this DataTable table, Func<DataRow, T> func)
        {
            if (table == null || table.Rows.Count == 0)
                return null;

            var list = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                var item = func(row);
                list.Add(item);
            }

            return list;
        }

        public static List<Dictionary<string, object>> ToDictionaryList(this DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
                return null;

            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in table.Rows)
            {
                var result = new Dictionary<string, object>();
                foreach (DataColumn column in table.Columns)
                {
                    result.Add(column.ColumnName, row[column]);
                }
                list.Add(result);
            }
            return list;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this DataTable table, string keyColumn, string valueColumn)
        {
            if (table == null || table.Rows.Count == 0)
                return null;

            var dictionary = new Dictionary<TKey, TValue>();
            foreach (DataRow row in table.Rows)
            {
                dictionary.Add(row.Get<TKey>(keyColumn), row.Get<TValue>(valueColumn));
            }

            return dictionary;
        }

        public static T ToEntity<T>(this DataRow row, Func<DataRow, T> func)
        {
            if (row == null)
                return default(T);

            return func(row);
        }

        public static T Get<T>(this DataRow row, string columnName)
        {
            if (row == null)
                return default(T);

            if (!row.Table.Columns.Contains(columnName))
                return default(T);

            return row[columnName].To<T>();
        }

        public static T Get<T>(this DbDataReader reader, string columnName)
        {
            if (reader == null)
                return default(T);

            return reader[columnName].To<T>();
        }
    }
}
