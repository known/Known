using System;
using System.Collections.Generic;
using System.Data;
using Known.Extensions;
using Known.Mapping;

namespace Known.Data
{
    /// <summary>
    /// 数据库访问类。
    /// </summary>
    public class Database : IDisposable
    {
        private IDbProvider provider;
        private List<Command> commands = new List<Command>();

        /// <summary>
        /// 构造函数，创建数据访问实例。
        /// </summary>
        /// <param name="provider">数据库提供者对象。</param>
        public Database(IDbProvider provider)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
            ConnectionString = provider.ConnectionString;
        }

        /// <summary>
        /// 取得数据库连接字符串。
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// 取得或设置当前用户账号。
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 执行增删改SQL语句。
        /// </summary>
        /// <param name="sql">增删改SQL语句。</param>
        /// <param name="param">SQL语句参数。</param>
        public void Execute(string sql, dynamic param = null)
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
        public T Scalar<T>(string sql, dynamic param = null)
        {
            var command = CommandCache.GetCommand(sql, param);
            return (T)provider.Scalar(command);
        }

        /// <summary>
        /// 执行查询SQL语句，返回指定类型的单个对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">查询SQL语句。</param>
        /// <param name="param">SQL语句参数。</param>
        /// <returns>指定类型的单个对象。</returns>
        public T Query<T>(string sql, dynamic param = null) where T : EntityBase
        {
            var row = QueryRow(sql, param);
            if (row == null)
                return default(T);

            return GetEntity<T>(row);
        }

        /// <summary>
        /// 执行查询SQL语句，返回指定类型的对象列表。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">查询SQL语句。</param>
        /// <param name="param">SQL语句参数。</param>
        /// <returns>指定类型的对象列表。</returns>
        public List<T> QueryList<T>(string sql, dynamic param = null) where T : EntityBase
        {
            var data = QueryTable(sql, param);
            return GetEntities(data);
        }

        /// <summary>
        /// 执行分页查询SQL语句，返回指定类型的分页查询结果。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="sql">分页查询SQL语句。</param>
        /// <param name="criteria">分页查询条件。</param>
        /// <returns>指定类型的分页查询结果。</returns>
        public PagingResult<T> QueryPage<T>(string sql, PagingCriteria criteria)
        {
            var cmd = CommandCache.GetCommand(sql, criteria.Parameters);
            if (cmd == null)
                return null;

            var sqlCount = CommandCache.GetCountSql(cmd.Text);
            var cmdCount = new Command(sqlCount, cmd.Parameters);
            var totalCount = (int)provider.Scalar(cmdCount);
            
            var sqlPage = CommandCache.GetPagingSql(cmd.Text, criteria);
            var cmdData = new Command(sqlPage, cmd.Parameters);
            var data = provider.Query(cmdData);
            var pageData = GetEntities<T>(data);
            return new PagingResult<T>(totalCount, pageData);
        }

        /// <summary>
        /// 保存实体对象。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="entity">实体对象。</param>
        public void Save<T>(T entity) where T : EntityBase
        {
            if (entity.IsNew)
            {
                entity.CreateBy = UserName;
                entity.CreateTime = DateTime.Now;
            }
            else
            {
                entity.ModifyBy = UserName;
                entity.ModifyTime = DateTime.Now;
            }

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
                provider.Execute(commands);
            }
            else if (commands.Count > 0)
            {
                provider.Execute(commands[0]);
            }
            commands.Clear();
        }

        /// <summary>
        /// 将整表数据写入数据库，表名及栏位名需与数据库一致。
        /// </summary>
        /// <param name="table">数据表。</param>
        public void WriteTable(DataTable table)
        {
            provider.WriteTable(table);
        }

        /// <summary>
        /// 执行查询SQL语句，返回数据表。
        /// </summary>
        /// <param name="sql">查询SQL语句。</param>
        /// <param name="param">SQL语句参数。</param>
        /// <returns>数据表。</returns>
        public DataTable QueryTable(string sql, dynamic param = null)
        {
            var command = CommandCache.GetCommand(sql, param);
            return provider.Query(command);
        }

        /// <summary>
        /// 执行查询SQL语句，返回数据行。
        /// </summary>
        /// <param name="sql">查询SQL语句。</param>
        /// <param name="param">SQL语句参数。</param>
        /// <returns>数据行。</returns>
        public DataRow QueryRow(string sql, dynamic param = null)
        {
            var command = CommandCache.GetCommand(sql, param);
            var data = provider.Query(command);
            if (data == null || data.Rows.Count == 0)
                return null;

            return data.Rows[0];
        }

        /// <summary>
        /// 根据表名及参数查询数据表。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="parameters">命令参数字典。</param>
        /// <returns>数据表。</returns>
        public DataTable SelectTable(string tableName, Dictionary<string, object> parameters = null)
        {
            var command = CommandCache.GetSelectCommand(tableName, parameters);
            return provider.Query(command);
        }

        /// <summary>
        /// 根据表名及参数查询数据行。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="parameters">命令参数字典。</param>
        /// <returns>数据行。</returns>
        public DataRow SelectRow(string tableName, Dictionary<string, object> parameters)
        {
            var data = SelectTable(tableName, parameters);
            if (data == null || data.Rows.Count == 0)
                return null;

            return data.Rows[0];
        }

        /// <summary>
        /// 根据表名及参数插入数据。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="parameters">命令参数字典。</param>
        public void Insert(string tableName, Dictionary<string, object> parameters)
        {
            var command = CommandCache.GetInsertCommand(tableName, parameters);
            commands.Add(command);
        }

        /// <summary>
        /// 根据表名及参数修改数据。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="keyFields">主键字段名，多个用“,”分割。</param>
        /// <param name="parameters">命令参数字典。</param>
        public void Update(string tableName, string keyFields, Dictionary<string, object> parameters)
        {
            var command = CommandCache.GetUpdateCommand(tableName, keyFields, parameters);
            commands.Add(command);
        }

        /// <summary>
        /// 根据表名及参数删除数据。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="parameters">命令参数字典。</param>
        public void Delete(string tableName, Dictionary<string, object> parameters)
        {
            var command = CommandCache.GetDeleteCommand(tableName, parameters);
            commands.Add(command);
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            if (commands.Count > 0)
            {
                SubmitChanges();
            }
        }

        private static T GetBaseEntity<T>(DataRow row) where T : EntityBase
        {
            var entity = GetEntity<T>(row);
            entity.IsNew = false;
            return entity;
        }

        private static List<T> GetBaseEntities<T>(DataTable data) where T : EntityBase
        {
            if (data == null || data.Rows.Count == 0)
                return null;

            var lists = new List<T>();
            foreach (DataRow row in data.Rows)
            {
                lists.Add(GetBaseEntity<T>(row));
            }
            return lists;
        }

        private static T GetEntity<T>(DataRow row)
        {
            if (row == null)
                return default(T);

            var entity = Activator.CreateInstance<T>();
            var properties = typeof(T).GetColumnProperties();
            foreach (var property in properties)
            {
                var columnName = ColumnInfo.GetColumnName(property);
                if (row.Table.Columns.Contains(columnName))
                {
                    var value = GetPropertyValue(property.PropertyType, row[columnName]);
                    property.SetValue(entity, value, null);
                }
            }
            
            return entity;
        }

        private static List<T> GetEntities<T>(DataTable data)
        {
            if (data == null || data.Rows.Count == 0)
                return null;

            var lists = new List<T>();
            foreach (DataRow row in data.Rows)
            {
                lists.Add(GetEntity<T>(row));
            }
            return lists;
        }

        private static object GetPropertyValue(Type type, object value)
        {
            if (type.IsSubclassOf(typeof(EntityBase)))
            {
                var entity = Activator.CreateInstance(type) as EntityBase;
                entity.Id = value.ToString();
                entity.IsNew = false;
                return entity;
            }

            return value;
        }
    }
}
