using System.Collections.Generic;
using Known.Core.Entities;

namespace Known.Core.Repositories
{
    public interface IOrganizationRepository
    {
        PagingResult<SysOrganization> QueryOrganizations(Database db, PagingCriteria criteria);
        List<SysOrganization> GetOrganizations(Database db);
        bool ExistsOrganization(Database db, SysOrganization entity);
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

        public bool ExistsOrganization(Database db, SysOrganization entity)
        {
            var sql = "select count(*) from SysOrganization where Id<>@Id and CompNo=@CompNo and Code=@Code";
            return db.Scalar<int>(sql, new { entity.Id, entity.CompNo, entity.Code }) > 0;
        }

        public bool ExistsSubOrganization(Database db, string parentId)
        {
            var sql = "select count(*) from SysOrganization where ParentId=@parentId";
            return db.Scalar<int>(sql, new { parentId }) > 0;
        }
    }
}
