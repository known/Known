using System;
using System.Collections.Generic;
using Known.Mapping;

namespace Known.Data
{
    /// <summary>
    /// 基本的数据仓库接口实现类。
    /// </summary>
    public class DbRepository : IRepository
    {
        /// <summary>
        /// 创建一个基本的数据仓库类的实例。
        /// </summary>
        /// <param name="database">数据库访问对象。</param>
        protected DbRepository(Database database)
        {
            Database = database;
        }

        /// <summary>
        /// 取得数据库访问对象。
        /// </summary>
        protected Database Database { get; }

        /// <summary>
        /// 根据 id 查询指定泛型的实体对象。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="id">实体主键。</param>
        /// <returns>实体对象。</returns>
        public T QueryById<T>(string id) where T : EntityBase
        {
            if (string.IsNullOrWhiteSpace(id))
                return default;

            return Database.QueryById<T>(id);
        }

        /// <summary>
        /// 根据指定泛型查询该泛型所有实体对象集合。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <returns>实体对象集合。</returns>
        public List<T> QueryList<T>() where T : EntityBase
        {
            return Database.QueryList<T>();
        }

        /// <summary>
        /// 根据主键数组查询指定泛型的实体对象集合。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="ids">实体主键数组。</param>
        /// <returns>实体对象集合。</returns>
        public List<T> QueryListById<T>(string[] ids) where T : EntityBase
        {
            if (ids == null || ids.Length == 0)
                return null;

            return Database.QueryListById<T>(ids);
        }

        /// <summary>
        /// 保存指定泛型的实体对象。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="entity">实体对象。</param>
        public void Save<T>(T entity) where T : EntityBase
        {
            if (entity == null)
                return;

            Database.Save(entity);
        }

        /// <summary>
        /// 删除指定泛型的实体对象。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="entity">实体对象。</param>
        public void Delete<T>(T entity) where T : EntityBase
        {
            if (entity == null)
                return;

            Database.Delete(entity);
        }

        /// <summary>
        /// 数据仓库事务操作。
        /// </summary>
        /// <param name="name">事务操作名。</param>
        /// <param name="action">事务操作方法。</param>
        /// <param name="data">操作结果返回的数据对象。</param>
        /// <returns>操作结果。</returns>
        public Result Transaction(string name, Action<IRepository> action, object data = null)
        {
            var db = new Database(Database.Name);
            var rep = new DbRepository(db);

            try
            {
                rep.Database.BeginTrans();
                action(rep);
                rep.Database.Commit();
                return Result.Success($"{name}成功！", data);
            }
            catch (Exception ex)
            {
                rep.Database.Rollback();
                throw ex;
            }
            finally
            {
                rep.Database.Dispose();
            }
        }
    }
}
