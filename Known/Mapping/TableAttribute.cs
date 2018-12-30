using System;

namespace Known.Mapping
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public TableAttribute(string tableName, string description, string primaryKey = "id")
        {
            TableName = tableName;
            Description = description;
            PrimaryKey = primaryKey;
        }

        public string TableName { get; }
        public string Description { get; }
        public string PrimaryKey { get; }
        
        public string[] PrimaryKeys
        {
            get { return PrimaryKey.Split(','); }
        }
    }
}
