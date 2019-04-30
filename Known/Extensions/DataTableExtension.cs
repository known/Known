using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Known.Extensions
{
    public static class DataTableExtension
    {
        public static DataTable ToPageTable(this DataTable table, int pageIndex, int pageSize)
        {
            if (table == null || table.Rows.Count == 0)
                return null;

            return table.AsEnumerable()
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .CopyToDataTable();
        }

        public static bool ColumnSameAs(this DataTable table, DataTable compare, out string error)
        {
            if (table.Columns.Count != compare.Columns.Count)
            {
                error = "栏位条数不一致！";
                return false;
            }

            var names = new List<string>();
            var count = 0;
            foreach (DataColumn item in table.Columns)
            {
                if (compare.Columns.Contains(item.ColumnName))
                    count += 1;
                else
                    names.Add(item.ColumnName);
            }

            error = string.Empty;
            if (names.Count > 0)
                error = string.Format("不存在【{0}】这些栏位！", string.Join(",", names));

            return count == table.Columns.Count;
        }

        public static T Get<T>(this DataRow row, string columnName, T defaultValue = default)
        {
            if (row.Table.Columns.Contains(columnName))
            {
                return Utils.ConvertTo<T>(row[columnName], defaultValue);
            }
            return defaultValue;
        }

        public static List<string> GetDuplicateValues(this DataTable table, string[] fields, string split = ",")
        {
            if (fields == null || fields.Length == 0)
                return new List<string>();

            return table.AsEnumerable()
                        .GroupBy(r => string.Join(split, fields.Select(f => r.Get<string>(f))))
                        .Select(r => new { r.Key, Count = r.Count() })
                        .Where(r => r.Count > 1)
                        .Select(r => r.Key)
                        .ToList();
        }
    }
}
