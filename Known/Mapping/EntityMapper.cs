using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Known.Mapping
{
    public abstract class EntityMapper<T> where T : EntityBase
    {
        public EntityMapper(string tableName, string description, string primaryKey = "id")
        {
            Table = new TableAttribute(tableName, description, primaryKey);
            Columns = new List<ColumnInfo>();

            this.Property(p => p.Id)
                .IsStringColumn("id", "主键", 1, 50, true);

            this.Property(p => p.CreateBy)
                .IsStringColumn("create_by", "创建人", 1, 50, true);

            this.Property(p => p.CreateTime)
                .IsDateTimeColumn("create_time", "创建时间", true);

            this.Property(p => p.ModifyBy)
                .IsStringColumn("modify_by", "修改人", 1, 50);

            this.Property(p => p.ModifyTime)
                .IsDateTimeColumn("modify_time", "修改时间");

            this.Property(p => p.Extension)
                .IsStringColumn("extension", "扩展属性");
        }

        public TableAttribute Table { get; }
        public List<ColumnInfo> Columns { get; }

        protected PropertyMapper Property<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var body = expression.Body.ToString();
            var name = body.Substring(body.LastIndexOf(".") + 1);
            var property = typeof(T).GetProperty(name);
            var mapper = new PropertyMapper(property, Table.PrimaryKeys);
            Columns.Add(mapper.Info);
            return mapper;
        }
    }
}
