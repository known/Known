using System.Collections.Generic;
using System.Data;

namespace Known.Data
{
    public class DataRowInfo : Dictionary<string, object>
    {
        public DataRowInfo(string tableName, string keyFields)
            : this(tableName, keyFields, true)
        {
        }

        private DataRowInfo(string tableName, string keyFields, bool isNew)
        {
            TableName = tableName;
            KeyFields = keyFields;
            IsNew = isNew;
        }

        public string TableName { get; }
        public string KeyFields { get; }
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
