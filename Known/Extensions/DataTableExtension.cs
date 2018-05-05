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
        /// 比较两个数据表的栏位是否一致。
        /// </summary>
        /// <param name="table">数据表。</param>
        /// <param name="compare">比较的数据表。</param>
        /// <param name="error">错误信息。</param>
        /// <returns>一致则返回true，否则返回false。</returns>
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
        /// 获取数据表的分页数据表。
        /// </summary>
        /// <param name="table">数据表。</param>
        /// <param name="pageIndex">页码。</param>
        /// <param name="pageSize">没页大小。</param>
        /// <returns>当前页码的数据表。</returns>
        public static DataTable ToPageTable(this DataTable table, int pageIndex, int pageSize)
        {
            if (table == null || table.Rows.Count == 0)
                return null;

            return table.AsEnumerable()
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .CopyToDataTable();
        }

        /// <summary>
        /// 根据栏位名获取指定类型的栏位数据。
        /// </summary>
        /// <typeparam name="T">栏位数据类型。</typeparam>
        /// <param name="row">数据行。</param>
        /// <param name="columnName">栏位名。</param>
        /// <param name="defaultValue">无数据时返回的默认值。</param>
        /// <returns>指定类型的栏位数据。</returns>
        public static T Get<T>(this DataRow row, string columnName, T defaultValue = default(T))
        {
            if (row.Table.Columns.Contains(columnName))
            {
                return Utils.ConvertTo<T>(row[columnName], defaultValue);
            }
            return defaultValue;
        }
    }
}
