namespace Known.Core.Repositories;

class OrganizationRepository
{
    internal static PagingResult<SysOrganization> QueryOrganizations(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysOrganization where AppId=@AppId and CompNo=@CompNo";
        return db.QueryPage<SysOrganization>(sql, criteria);
    }

    internal static List<SysOrganization> GetOrganizations(Database db, string appId, string compNo)
    {
        var sql = "select * from SysOrganization where AppId=@appId and CompNo=@compNo order by ParentId,Code";
        return db.QueryList<SysOrganization>(sql, new { appId, compNo });
    }

    internal static SysOrganization GetOrganization(Database db, string appId, string compNo, string code)
    {
        var sql = "select * from SysOrganization where AppId=@appId and CompNo=@compNo and Code=@code";
        return db.Query<SysOrganization>(sql, new { appId, compNo, code });
    }

    internal static bool ExistsOrganization(Database db, SysOrganization entity)
    {
        var sql = "select count(*) from SysOrganization where Id<>@Id and AppId=@AppId and CompNo=@CompNo and Code=@Code";
        return db.Scalar<int>(sql, new { entity.Id, entity.AppId, entity.CompNo, entity.Code }) > 0;
    }

    internal static bool ExistsSubOrganization(Database db, string parentId)
    {
        var sql = "select count(*) from SysOrganization where ParentId=@parentId";
        return db.Scalar<int>(sql, new { parentId }) > 0;
    }
}