using System.Collections.Generic;
using System.Data;

namespace Known.Data
{
    /// <summary>
    /// 数据行信息类。
    /// </summary>
    public class DataRowInfo : Dictionary<string, object>
    {
        internal DataRowInfo(string tableName, string keyFields)
            : this(tableName, keyFields, true)
        {
        }

        private DataRowInfo(string tableName, string keyFields, bool isNew)
        {
            TableName = tableName;
            KeyFields = keyFields;
            IsNew = isNew;
        }

        /// <summary>
        /// 取得数据表名。
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// 取得关键字字段。
        /// </summary>
        public string KeyFields { get; }

        /// <summary>
        /// 取得是否为新增数据行。
        /// </summary>
        public bool IsNew { get; }

        internal static DataRowInfo Load(string tableName, string keyFields, DataRow row)
        {
            if (row == null)
                return new DataRowInfo(tableName, keyFields);

            var info = new DataRowInfo(tableName, keyFields, false);
            foreach (DataColumn item in row.Table.Columns)
            {
                info[item.ColumnName] = row[item];
            }

            return info;
        }
    }
}
