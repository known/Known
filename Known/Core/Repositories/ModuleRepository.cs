using System.Collections.Generic;
using Known.Data;

namespace Known.Core.Repositories
{
    /// <summary>
    /// 系统模块数据仓库接口。
    /// </summary>
    public interface IModuleRepository : IRepository
    {
        /// <summary>
        /// 查询模块分页数据对象。
        /// </summary>
        /// <param name="criteria">查询条件对象。</param>
        /// <returns>分页数据对象。</returns>
        PagingResult QueryModules(PagingCriteria criteria);

        /// <summary>
        /// 获取指定模块的所有子模块信息列表。
        /// </summary>
        /// <param name="parentId">上级模块ID。</param>
        /// <returns>子模块信息列表。</returns>
        List<Entities.Module> GetModules(string parentId);

        /// <summary>
        /// 获取指定顺序的同级模块信息对象。
        /// </summary>
        /// <param name="parentId">上级模块ID。</param>
        /// <param name="sort">模块顺序。</param>
        /// <returns>模块信息对象。</returns>
        Entities.Module GetModule(string parentId, int sort);

        /// <summary>
        /// 判断指定模块是否存在子模块。
        /// </summary>
        /// <param name="id">模块ID。</param>
        /// <returns>存在子模块返回 True，否则返回 False。</returns>
        bool ExistsChildren(string id);
    }

    internal class ModuleRepository : DbRepository, IModuleRepository
    {
        public ModuleRepository(Database database) : base(database)
        {
        }

        public PagingResult QueryModules(PagingCriteria criteria)
        {
            var sql = "select * from t_plt_modules where parent_id=@pid";

            var key = (string)criteria.Parameter.key;
            if (!string.IsNullOrWhiteSpace(key))
            {
                sql += " and (code like @key or name like @key)";
                criteria.Parameter.key = $"%{key}%";
            }

            return Database.QueryPage<Module>(sql, criteria);
        }

        public List<Entities.Module> GetModules(string parentId)
        {
            var sql = "select * from t_plt_modules where parent_id=@parentId order by sort";
            return Database.QueryList<Entities.Module>(sql, new { parentId });
        }

        public Entities.Module GetModule(string parentId, int sort)
        {
            var sql = "select * from t_plt_modules where parent_id=@parentId and sort=@sort";
            return Database.Query<Entities.Module>(sql, new { parentId, sort });
        }

        public bool ExistsChildren(string id)
        {
            var sql = "select count(*) from t_plt_modules where parent_id=@id";
            return Database.Scalar<int>(sql, new { id }) > 0;
        }
    }
}
