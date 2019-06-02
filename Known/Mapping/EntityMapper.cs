using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Known.Mapping
{
    /// <summary>
    /// 实体映射器抽象基类。
    /// </summary>
    public abstract class EntityMapper
    {
        /// <summary>
        /// 初始化一个实体映射器类的实例。
        /// </summary>
        /// <param name="type">实体类型。</param>
        /// <param name="tableName">数据库表名。</param>
        /// <param name="description">实体描述。</param>
        /// <param name="primaryKey">主键字段，默认 id。</param>
        public EntityMapper(Type type, string tableName, string description, string primaryKey = "id")
        {
            Type = type;
            Table = new TableAttribute(tableName, description, primaryKey);
            Columns = new List<ColumnInfo>();
        }

        /// <summary>
        /// 取得实体类型。
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// 取得实体表属性。
        /// </summary>
        public TableAttribute Table { get; }

        /// <summary>
        /// 取得实体所有栏位信息。
        /// </summary>
        public List<ColumnInfo> Columns { get; }
    }

    /// <summary>
    /// 实体映射器泛型抽象基类。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    public abstract class EntityMapper<T> : EntityMapper where T : EntityBase
    {
        /// <summary>
        /// 初始化一个实体映射器泛型类的实例。
        /// </summary>
        /// <param name="tableName">数据库表名。</param>
        /// <param name="description">实体描述。</param>
        /// <param name="primaryKey">主键字段，默认 id。</param>
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

        /// <summary>
        /// 根据表达式获取实体栏位属性映射器。
        /// </summary>
        /// <typeparam name="TProperty">属性类型。</typeparam>
        /// <param name="expression">获取属性表达式。</param>
        /// <returns>实体栏位属性映射器。</returns>
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
