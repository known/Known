using System.Collections.Generic;
using Known.Core.Entities;

namespace Known.Core.Datas
{
    public interface ISystemRepository
    {
        bool ExistsSubModule(Database db, string parentId);
        List<SysModule> GetModules(Database db);
        PagingResult<SysModule> QueryModules(Database db, PagingCriteria criteria);
    }

    class SystemRepository : ISystemRepository
    {
        public bool ExistsSubModule(Database db, string parentId)
        {
            var sql = "select count(*) from SysModule where ParentId=@parentId";
            return db.Scalar<int>(sql, new { parentId }) > 0;
        }

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
