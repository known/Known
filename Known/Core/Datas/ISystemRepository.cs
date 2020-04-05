using Known.Core.Entities;

namespace Known.Core.Datas
{
    public interface ISystemRepository
    {
        PagingResult<SysModule> QueryModules(Database db, PagingCriteria criteria);
    }

    class SystemRepository : ISystemRepository
    {
        public PagingResult<SysModule> QueryModules(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysModule";
            return db.QueryPage<SysModule>(sql, criteria);
        }
    }
}
