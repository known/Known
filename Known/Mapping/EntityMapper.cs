﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Known.Mapping
{
    public abstract class EntityMapper
    {
        public EntityMapper(Type type, string tableName, string description, string primaryKey = "id")
        {
            Type = type;
            Table = new TableAttribute(tableName, description, primaryKey);
            Columns = new List<ColumnInfo>();
        }

        public Type Type { get; }
        public TableAttribute Table { get; }
        public List<ColumnInfo> Columns { get; }
    }

    public abstract class EntityMapper<T> : EntityMapper where T : EntityBase
    {
        public EntityMapper(string tableName, string description, string primaryKey = "id")
            : base(typeof(T), tableName, description, primaryKey)
        {
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

        protected PropertyMapper Property<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var body = expression.Body.ToString();
            var name = body.Substring(body.LastIndexOf(".") + 1);
            var property = Type.GetProperty(name);
            var mapper = new PropertyMapper(property, Table.PrimaryKeys);
            Columns.Add(mapper.Info);
            return mapper;
        }
    }
}
