using System.Collections.Generic;
using Known.Core.Entities;

namespace Known.Core.Datas
{
    public interface ISystemRepository
    {
        List<SysModule> GetModules(Database db);
        PagingResult<SysModule> QueryModules(Database db, PagingCriteria criteria);
    }

    class SystemRepository : ISystemRepository
    {
        public List<SysModule> GetModules(Database db)
        {
            var sql = "select * from SysModule order by Sort";
            return db.QueryList<SysModule>(sql);
        }

        public PagingResult<SysModule> QueryModules(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysModule";
            return db.QueryPage<SysModule>(sql, criteria);
        }
    }
}
