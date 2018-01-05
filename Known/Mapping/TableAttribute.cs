using System;

namespace Known.Mapping
{
    /// <summary>
    /// 数据表特性，用于实体和表的映射。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public TableAttribute() { }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="tableName">数据表名。</param>
        /// <param name="primaryKey">数据表主键字段，多个用半角逗号分割。</param>
        public TableAttribute(string tableName, string primaryKey)
        {
            TableName = tableName;
            PrimaryKey = primaryKey;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="tableName">数据表名。</param>
        /// <param name="primaryKey">数据表主键字段，多个用半角逗号分割。</param>
        /// <param name="description">数据表描述。</param>
        public TableAttribute(string tableName, string primaryKey, string description)
        {
            TableName = tableName;
            PrimaryKey = primaryKey;
            Description = description;
        }

        /// <summary>
        /// 取得或这是数据表名。
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 取得或设置数据表主键字段，多个用半角逗号分割。
        /// </summary>
        public string PrimaryKey { get; set; }

        /// <summary>
        /// 取得或设置数据表描述。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 取得数据表主键字段数组。
        /// </summary>
        public string[] PrimaryKeys
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PrimaryKey))
                    return null;

                return PrimaryKey.Split(',');
            }
        }
    }
}
