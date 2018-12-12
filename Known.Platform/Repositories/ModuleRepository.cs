using System.Collections.Generic;
using Known.Data;

namespace Known.Platform.Repositories
{
    public interface IModuleRepository : IRepository
    {
        PagingResult<Module> QueryModules(PagingCriteria criteria);
        List<Module> GetModules(string[] ids);
    }

    internal class ModuleRepository : DbRepository, IModuleRepository
    {
        public ModuleRepository(Database database) : base(database)
        {
        }

        public PagingResult<Module> QueryModules(PagingCriteria criteria)
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

        public List<Module> GetModules(string[] ids)
        {
            var id = string.Join("','", ids);
            var sql = $"select * from t_plt_modules where id in ('{id}')";
            return Database.QueryList<Module>(sql);
        }
    }
}
