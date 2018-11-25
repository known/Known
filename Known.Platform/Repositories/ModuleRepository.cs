using System.Collections.Generic;
using Known.Data;

namespace Known.Platform.Repositories
{
    public interface IModuleRepository : IRepository
    {
        PagingResult<Module> QueryModules(string parentId, string key);
    }

    internal class ModuleRepository : DbRepository, IModuleRepository
    {
        public ModuleRepository(Database database) : base(database)
        {
        }

        public PagingResult<Module> QueryModules(string parentId, string key)
        {
            var sql = "select * from t_plt_modules where parent_id=@parentId";
            var modules = Database.QueryList<Module>(sql, new { parentId });
            return new PagingResult<Module>(10, modules);
        }
    }
}
