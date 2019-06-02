using System;
using System.Collections.Generic;
using System.Data;
using Known.Mapping;

namespace Known.Data
{
    /// <summary>
    /// 数据库访问操作类。
    /// </summary>
    public class Database : IDisposable
    {
        private IDbProvider provider;

        /// <summary>
        /// 初始化一个数据库访问操作类实例。
        /// </summary>
        public Database()
        {
            provider = Container.Resolve<IDbProvider>();

            if (provider == null)
            {
                Name = "Default";
                provider = new DbProvider(Name);
                ConnectionString = provider.ConnectionString;
            }
        }

        /// <summary>
        /// 初始化一个指定链接名称的数据库访问操作类实例。
        /// </summary>
        /// <param name="name">数据库链接名称。</param>
        public Database(string name)
        {
            Name = name;
            provider = new DbProvider(Name);
            ConnectionString = provider.ConnectionString;
        }

        /// <summary>
        /// 取得数据库链接名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 取得数据库访问链接字符串。
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// 取得当前用户名。
        /// </summary>
        public string UserName { get; internal set; }

        internal void BeginTrans()
        {
            provider.BeginTrans();
        }

        internal void Commit()
        {
            provider.Commit();
        }

        internal void Rollback()
        {
            provider.Rollback();
        }

        /// <summary>
        /// 数据仓库事务操作。
        /// </summary>
        /// <param name="name">事务操作名。</param>
        /// <param name="action">事务操作方法。</param>
        /// <param name="data">操作结果返回的数据对象。</param>
        /// <returns>操作结果。</returns>
        public Result Transaction(string name, Action<Database> action, object data = null)
        {
            var db = new Database(Name);

            try
            {
                db.BeginTrans();
                action(db);
                db.Commit();
                return Result.Success($"{name}成功！", data);
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// 执行非查询的数据库操作。
        /// </summary>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="param">SQL 参数。</param>
        public void Execute(string sql, object param = null)
        {
            var command = CommandHelper.GetCommand(sql, param);
            provider.Execute(command);
        }

        /// <summary>
        /// 执行查询标量的数据库操作。
        /// </summary>
        /// <typeparam name="T">标量值类型。</typeparam>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="param">SQL 参数。</param>
        /// <returns>标量值。</returns>
        public T Scalar<T>(string sql, object param = null)
        {
            var command = CommandHelper.GetCommand(sql, param);
            return (T)provider.Scalar(command);
        }

        /// <summary>
        /// 根据 id 查询指定泛型的实体对象。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="id">实体主键。</param>
        /// <returns>实体对象。</returns>
        public T QueryById<T>(string id) where T : EntityBase
        {
            var sql = CommandHelper.GetQueryByIdSql<T>();
            return Query<T>(sql, new { id });
        }

        /// <summary>
        /// 根据 SQL 语句查询实体。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="param">SQL 参数。</param>
        /// <returns>实体对象。</returns>
        public T Query<T>(string sql, object param = null) where T : EntityBase
        {
            var row = QueryRow(sql, param);
            if (row == null)
                return default;

            return AutoMapper.GetBaseEntity<T>(row);
        }

        /// <summary>
        /// 查询指定泛型的所有实体对象集合。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <returns>实体对象集合。</returns>
        public List<T> QueryList<T>() where T : EntityBase
        {
            var sql = CommandHelper.GetQueryListSql<T>();
            var data = QueryTable(sql);
            return AutoMapper.GetBaseEntities<T>(data);
        }

        /// <summary>
        /// 根据 SQL 语句查询实体对象集合。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="param">SQL 参数。</param>
        /// <returns>实体对象集合。</returns>
        public List<T> QueryList<T>(string sql, object param = null) where T : EntityBase
        {
            var data = QueryTable(sql, param);
            return AutoMapper.GetBaseEntities<T>(data);
        }

        /// <summary>
        /// 根据主键数组查询指定泛型的实体对象集合。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="ids">实体主键数组。</param>
        /// <returns>实体对象集合。</returns>
        public List<T> QueryListById<T>(string[] ids) where T : EntityBase
        {
            var sql = CommandHelper.GetQueryListByIdSql<T>(ids);
            var data = QueryTable(sql);
            return AutoMapper.GetBaseEntities<T>(data);
        }

        /// <summary>
        /// 根据 SQL 语句查询分页结果，返回实体集合。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="criteria">分页查询条件。</param>
        /// <returns>分页结果。</returns>
        public PagingResult QueryPage<T>(string sql, PagingCriteria criteria)
        {
            var result = QueryPageTable(sql, criteria);
            if (result == null)
                return null;

            var pageData = AutoMapper.GetEntities<T>(result.PageData as DataTable);
            return new PagingResult(result.TotalCount, pageData);
        }

        /// <summary>
        /// 保存实体对象，自动判断是否新增或修改。
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

            var command = CommandHelper.GetSaveCommand(entity);
            provider.Execute(command);
        }

        /// <summary>
        /// 修改实体对象。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="entity">实体对象。</param>
        public void Update<T>(T entity) where T : EntityBase
        {
            if (string.IsNullOrWhiteSpace(entity.Id))
                return;

            entity.IsNew = false;
            entity.ModifyBy = UserName;
            entity.ModifyTime = DateTime.Now;
            var command = CommandHelper.GetSaveCommand(entity);
            provider.Execute(command);
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
            var command = CommandHelper.GetDeleteCommand(entity);
            provider.Execute(command);
        }

        /// <summary>
        /// 删除实体对象集合。
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
        /// 批量将数据表数据写入数据库。
        /// </summary>
        /// <param name="table">数据表。</param>
        public void WriteTable(DataTable table)
        {
            provider.WriteTable(table);
        }

        /// <summary>
        /// 根据 SQL 语句查询数据表。
        /// </summary>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="param">SQL 参数。</param>
        /// <returns>数据表。</returns>
        public DataTable QueryTable(string sql, object param = null)
        {
            var command = CommandHelper.GetCommand(sql, param);
            return provider.Query(command);
        }

        /// <summary>
        /// 根据 SQL 语句查询分页结果，返回数据表。
        /// </summary>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="criteria">分页查询条件。</param>
        /// <returns>分页结果。</returns>
        public PagingResult QueryPageTable(string sql, PagingCriteria criteria)
        {
            var cmd = CommandHelper.GetQueryCommand(sql, criteria.Parameter);
            if (cmd == null)
                return null;

            var sqlCount = CommandHelper.GetCountSql(cmd.Text);
            var cmdCount = new Command(sqlCount, cmd.Parameters);
            var totalCount = (int)provider.Scalar(cmdCount);

            var sqlPage = CommandHelper.GetPagingSql(cmd.Text, criteria);
            var cmdData = new Command(sqlPage, cmd.Parameters);
            var pageData = provider.Query(cmdData);
            return new PagingResult(totalCount, pageData);
        }

        /// <summary>
        /// 根据 SQL 语句查询数据行。
        /// </summary>
        /// <param name="sql">SQL 语句。</param>
        /// <param name="param">SQL 参数。</param>
        /// <returns>数据行。</returns>
        public DataRow QueryRow(string sql, object param = null)
        {
            var command = CommandHelper.GetCommand(sql, param);
            var data = provider.Query(command);
            if (data == null || data.Rows.Count == 0)
                return null;

            return data.Rows[0];
        }

        /// <summary>
        /// 释放数据库访问操作类的资源。
        /// </summary>
        public void Dispose()
        {
            if (provider != null)
            {
                provider.Dispose();
                provider = null;
            }
        }
    }
}
