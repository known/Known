using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Known.Extensions
{
    /// <summary>
    /// 数据表扩展类。
    /// </summary>
    public static class DataTableExtension
    {
        /// <summary>
        /// 获取数据表的分页数据表。
        /// </summary>
        /// <param name="table">数据表对象。</param>
        /// <param name="pageIndex">页码。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <returns>分页数据表对象。</returns>
        public static DataTable ToPageTable(this DataTable table, int pageIndex, int pageSize)
        {
            if (table == null || table.Rows.Count == 0)
                return null;

            return table.AsEnumerable()
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .CopyToDataTable();
        }

        /// <summary>
        /// 判断两个数据表的栏位是否相同。
        /// </summary>
        /// <param name="table">数据表对象。</param>
        /// <param name="compare">比对数据表对象。</param>
        /// <param name="error">返回的错误消息。</param>
        /// <returns>栏位是否相同。</returns>
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

        /// <summary>
        /// 获取数据行中指定类型的栏位数据。
        /// </summary>
        /// <typeparam name="T">栏位数据类型。</typeparam>
        /// <param name="row">数据行对象。</param>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="defaultValue">为空时的默认值。</param>
        /// <returns>栏位数据。</returns>
        public static T Get<T>(this DataRow row, string columnName, T defaultValue = default)
        {
            if (row.Table.Columns.Contains(columnName))
            {
                return Utils.ConvertTo<T>(row[columnName], defaultValue);
            }
            return defaultValue;
        }

        /// <summary>
        /// 获取数据表中重复栏位数据集合。
        /// </summary>
        /// <param name="table">数据表对象。</param>
        /// <param name="fields">栏位名称数组。</param>
        /// <param name="split">栏位数据分隔符，默认半角逗号。</param>
        /// <returns>重复栏位数据集合。</returns>
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
