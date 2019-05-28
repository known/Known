using System;
using System.Collections.Generic;
using Known.Mapping;

namespace Known.Data
{
    /// <summary>
    /// 基本的数据仓库接口。
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// 根据 id 查询指定泛型的实体对象。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="id">实体主键。</param>
        /// <returns>实体对象。</returns>
        T QueryById<T>(string id) where T : EntityBase;

        /// <summary>
        /// 根据指定泛型查询该泛型所有实体对象集合。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <returns>实体对象集合。</returns>
        List<T> QueryList<T>() where T : EntityBase;

        /// <summary>
        /// 根据主键数组查询指定泛型的实体对象集合。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="ids">实体主键数组。</param>
        /// <returns>实体对象集合。</returns>
        List<T> QueryListById<T>(string[] ids) where T : EntityBase;

        /// <summary>
        /// 保存指定泛型的实体对象。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="entity">实体对象。</param>
        void Save<T>(T entity) where T : EntityBase;

        /// <summary>
        /// 删除指定泛型的实体对象。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="entity">实体对象。</param>
        void Delete<T>(T entity) where T : EntityBase;

        /// <summary>
        /// 数据仓库事务操作。
        /// </summary>
        /// <param name="name">事务操作名。</param>
        /// <param name="action">事务操作方法。</param>
        /// <param name="data">操作结果返回的数据对象。</param>
        /// <returns>操作结果。</returns>
        Result Transaction(string name, Action<IRepository> action, object data = null);
    }
}
