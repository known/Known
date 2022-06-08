using System.Collections.Generic;

namespace Known.Core
{
    partial class PlatformService
    {
        public List<OrganizationInfo> GetOrganizations(string appId, string compNo)
        {
            return Repository.GetOrganizations(Database, appId, compNo);
        }

        public OrganizationInfo GetOrganization(string appId, string compNo, string code)
        {
            return Repository.GetOrganization(Database, appId, compNo, code);
        }
    }

    partial interface IPlatformRepository
    {
        List<OrganizationInfo> GetOrganizations(Database db, string appId, string compNo);
        OrganizationInfo GetOrganization(Database db, string appId, string compNo, string code);
    }

    partial class PlatformRepository
    {
        public List<OrganizationInfo> GetOrganizations(Database db, string appId, string compNo)
        {
            var sql = "select * from SysOrganization where AppId=@appId and CompNo=@compNo order by Code";
            return db.QueryList<OrganizationInfo>(sql, new { appId, compNo });
        }

        public OrganizationInfo GetOrganization(Database db, string appId, string compNo, string code)
        {
            var sql = "select * from SysOrganization where AppId=@appId and CompNo=@compNo and Code=@code";
            return db.Query<OrganizationInfo>(sql, new { appId, compNo, code });
        }
    }
}
