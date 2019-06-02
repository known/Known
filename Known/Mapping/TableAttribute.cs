using System;

namespace Known.Mapping
{
    /// <summary>
    /// 数据表特性类。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// 初始化一个数据表特性类的实例。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="description">描述。</param>
        /// <param name="primaryKey">主键，多个主键用半角逗号分隔，默认 id。</param>
        public TableAttribute(string tableName, string description, string primaryKey = "id")
        {
            TableName = tableName;
            Description = description;
            PrimaryKey = primaryKey;
        }

        /// <summary>
        /// 取得表名。
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// 取得描述。
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 取得主键。
        /// </summary>
        public string PrimaryKey { get; }

        /// <summary>
        /// 取得主键数组。
        /// </summary>
        public string[] PrimaryKeys
        {
            get { return PrimaryKey.Split(','); }
        }
    }
}
