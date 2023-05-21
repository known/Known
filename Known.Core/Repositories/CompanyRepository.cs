namespace Known.Core.Repositories;

class CompanyRepository
{
    //Company
    internal static SysCompany GetCompany(Database db, string code)
    {
        var sql = "select * from SysCompany where Code=@code";
        return db.Query<SysCompany>(sql, new { code });
    }

    //Organization
    internal static List<SysOrganization> GetOrganizations(Database db)
    {
        var sql = "select * from SysOrganization where CompNo=@CompNo";
        return db.QueryList<SysOrganization>(sql, new { db.User.CompNo });
    }

    internal static bool ExistsSubOrganization(Database db, string id)
    {
        var sql = "select count(*) from SysOrganization where ParentId=@id";
        return db.Scalar<int>(sql, new { id }) > 0;
    }

    internal static bool ExistsOrganization(Database db, SysOrganization entity)
    {
        var sql = "select count(*) from SysOrganization where Id<>@Id and CompNo=@CompNo and Code=@Code";
        return db.Scalar<int>(sql, new { entity.Id, entity.CompNo, entity.Code }) > 0;
    }
}