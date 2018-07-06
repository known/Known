using System;

namespace Known.Mapping
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public TableAttribute() { }

        public TableAttribute(string tableName, string primaryKey)
        {
            TableName = tableName;
            PrimaryKey = primaryKey;
        }

        public TableAttribute(string tableName, string primaryKey, string description)
        {
            TableName = tableName;
            PrimaryKey = primaryKey;
            Description = description;
        }

        public string TableName { get; set; }
        public string PrimaryKey { get; set; }
        public string Description { get; set; }

        public string[] PrimaryKeys
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PrimaryKey))
                    return new string[] { "Id" };

                return PrimaryKey.Split(',');
            }
        }
    }
}
