using System.Collections.Generic;
using Known.Core.Entities;

namespace Known.Core.Repositories
{
    public interface IOrganizationRepository
    {
        PagingResult<SysOrganization> QueryOrganizations(Database db, PagingCriteria criteria);
        List<SysOrganization> GetOrganizations(Database db);
        bool ExistsSubOrganization(Database db, string parentId);
    }

    class OrganizationRepository : IOrganizationRepository
    {
        public PagingResult<SysOrganization> QueryOrganizations(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysOrganization";
            return db.QueryPage<SysOrganization>(sql, criteria);
        }

        public List<SysOrganization> GetOrganizations(Database db)
        {
            var sql = "select * from SysOrganization order by ParentId,Code";
            return db.QueryList<SysOrganization>(sql);
        }

        public bool ExistsSubOrganization(Database db, string parentId)
        {
            var sql = "select count(*) from SysOrganization where ParentId=@parentId";
            return db.Scalar<int>(sql, new { parentId }) > 0;
        }
    }
}
