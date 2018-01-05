using Known.Extensions;
using Known.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Known.Data
{
    /// <summary>
    /// 数据访问类。
    /// </summary>
    public class Database : IDisposable
    {
        private IDatabase database;
        private List<Command> commands = new List<Command>();

        /// <summary>
        /// 构造函数，创建数据访问实例。
        /// </summary>
        /// <param name="database">数据库对象。</param>
        public Database(IDatabase database)
        {
            this.database = database ?? throw new ArgumentNullException("database");
            Type = database.Type;
            ConnectionString = database.ConnectionString;
        }

        /// <summary>
        /// 取得数据库类型。
        /// </summary>
        public DatabaseType Type { get; }

        /// <summary>
        /// 取得数据库连接字符串。
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// 执行增删改SQL语句。
        /// </summary>
        /// <param name="sql">增删改SQL语句。</param>
        /// <param name="param">SQL语句参数。</param>
        public void Execute(string sql, object param = null)
        {
            var command = CommandCache.GetCommand(sql, param);
            commands.Add(command);
        }

        /// <summary>
        /// 执行查询SQL语句，返回指定类型的标量。
        /// </summary>
        /// <typeparam name="T">标量类型。</typeparam>
        /// <param name="sql">查询SQL语句。</param>
        /// <param name="param">SQL语句参数。</param>
        /// <returns>指定类型的标量。</returns>
        public T Scalar<T>(string sql, object param = null)
        {
            var command = CommandCache.GetCommand(sql, param);
            return (T)database.Scalar(command);
        }

        /// <summary>
        /// 执行查询SQL语句，返回指定类型的单个对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">查询SQL语句。</param>
        /// <param name="param">SQL语句参数。</param>
        /// <returns>指定类型的单个对象。</returns>
        public T Query<T>(string sql, object param = null) where T : EntityBase
        {
            var command = CommandCache.GetCommand(sql, param);
            var data = database.Query(command);
            if (data == null || data.Rows.Count == 0)
                return default(T);

            return GetEntity<T>(data.Rows[0]);
        }

        /// <summary>
        /// 执行查询SQL语句，返回指定类型的对象列表。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">查询SQL语句。</param>
        /// <param name="param">SQL语句参数。</param>
        /// <returns>指定类型的对象列表。</returns>
        public List<T> QueryList<T>(string sql, object param = null) where T : EntityBase
        {
            var command = CommandCache.GetCommand(sql, param);
            var data = database.Query(command);
            if (data == null || data.Rows.Count == 0)
                return null;

            return data.AsEnumerable().Select(r => GetEntity<T>(r)).ToList();
        }

        /// <summary>
        /// 保存实体对象。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="entity">实体对象。</param>
        public void Save<T>(T entity) where T : EntityBase
        {
            var command = CommandCache.GetSaveCommand(entity);
            commands.Add(command);
        }

        /// <summary>
        /// 保存实体对象集合。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="entities">实体对象集合。</param>
        public void Save<T>(List<T> entities) where T : EntityBase
        {
            foreach (var entity in entities)
            {
                Save(entity);
            }
        }

        /// <summary>
        /// 删除实体对象。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="entity">实体对象。</param>
        public void Delete<T>(T entity) where T : EntityBase
        {
            var command = CommandCache.GetDeleteCommand(entity);
            commands.Add(command);
        }

        /// <summary>
        /// 删除多个实体对象。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="entities">实体对象集合。</param>
        public void Delete<T>(List<T> entities) where T : EntityBase
        {
            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }

        /// <summary>
        /// 提交改变的数据操作至数据库。
        /// </summary>
        public void SubmitChanges()
        {
            if (commands.Count > 1)
            {
                database.Execute(commands);
            }
            else if (commands.Count > 0)
            {
                database.Execute(commands[0]);
            }
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            commands.Clear();
        }

        private static T GetEntity<T>(DataRow row) where T : EntityBase
        {
            if (row == null)
                return default(T);

            var entity = Activator.CreateInstance<T>();
            var properties = typeof(T).GetColumnProperties();
            foreach (var property in properties)
            {
                var fieldName = ColumnInfo.GetColumnName(property);
                if (row.Table.Columns.Contains(fieldName))
                {
                    var value = GetPropertyValue(property.PropertyType, row[fieldName]);
                    property.SetValue(entity, value, null);
                }
            }
            entity.IsNew = false;
            return entity;
        }

        private static object GetPropertyValue(Type type, object value)
        {
            if (type.IsSubclassOf(typeof(EntityBase)))
            {
                var entity = Activator.CreateInstance(type);
                return entity;
            }

            return value;
        }
    }
}
