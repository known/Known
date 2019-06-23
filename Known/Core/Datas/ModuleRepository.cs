using System.Collections.Generic;
using Known.Data;

namespace Known.Core
{
    internal static class ModuleRepository
    {
        public static PagingResult QueryModules(this Database database, PagingCriteria criteria)
        {
            var sql = "select * from t_plt_modules where parent_id=@pid";

            var key = (string)criteria.Parameter.key;
            if (!string.IsNullOrWhiteSpace(key))
            {
                sql += " and (code like @key or name like @key)";
                criteria.Parameter.key = $"%{key}%";
            }

            return database.QueryPage<Module>(sql, criteria);
        }

        public static List<Module> GetModules(this Database database, string parentId)
        {
            var sql = "select * from t_plt_modules where parent_id=@parentId order by sort";
            return database.QueryList<Module>(sql, new { parentId });
        }

        public static Module GetModule(this Database database, string parentId, int sort)
        {
            var sql = "select * from t_plt_modules where parent_id=@parentId and sort=@sort";
            return database.Query<Module>(sql, new { parentId, sort });
        }

        public static bool ExistsChildren(this Database database, string id)
        {
            var sql = "select count(*) from t_plt_modules where parent_id=@id";
            return database.Scalar<int>(sql, new { id }) > 0;
        }
    }
}
