using System.Collections.Generic;
using Known.Core.Entities;
using Known.Data;

namespace Known.Core.Repositories
{
    public interface IModuleRepository : IRepository
    {
        PagingResult QueryModules(PagingCriteria criteria);
        List<Module> GetModules(string parentId);
        Module GetModule(string parentId, int sort);
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

        public List<Module> GetModules(string parentId)
        {
            var sql = "select * from t_plt_modules where parent_id=@parentId order by sort";
            return Database.QueryList<Module>(sql, new { parentId });
        }

        public Module GetModule(string parentId, int sort)
        {
            var sql = "select * from t_plt_modules where parent_id=@parentId and sort=@sort";
            return Database.Query<Module>(sql, new { parentId, sort });
        }

        public bool ExistsChildren(string id)
        {
            var sql = "select count(*) from t_plt_modules where parent_id=@id";
            return Database.Scalar<int>(sql, new { id }) > 0;
        }
    }
}
