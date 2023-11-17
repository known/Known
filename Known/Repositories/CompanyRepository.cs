using Known.Entities;

namespace Known.Repositories;

class CompanyRepository
{
    //Company
    internal static Task<SysCompany> GetCompanyAsync(Database db)
    {
        var sql = "select * from SysCompany where Code=@code";
        return db.QueryAsync<SysCompany>(sql, new { code = db.User.CompNo });
    }

    //Organization
    internal static Task<List<SysOrganization>> GetOrganizationsAsync(Database db)
    {
        var sql = "select * from SysOrganization where CompNo=@CompNo";
        return db.QueryListAsync<SysOrganization>(sql, new { db.User.CompNo });
    }

    internal static async Task<bool> ExistsSubOrganizationAsync(Database db, string id)
    {
        var sql = "select count(*) from SysOrganization where ParentId=@id";
        return await db.ScalarAsync<int>(sql, new { id }) > 0;
    }

    internal static async Task<bool> ExistsOrganizationAsync(Database db, SysOrganization entity)
    {
        var sql = "select count(*) from SysOrganization where Id<>@Id and CompNo=@CompNo and Code=@Code";
        return await db.ScalarAsync<int>(sql, new { entity.Id, entity.CompNo, entity.Code }) > 0;
    }
}