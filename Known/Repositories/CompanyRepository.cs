namespace Known.Repositories;

class CompanyRepository
{
    //Company
    internal static Task<SysCompany> GetCompanyAsync(Database db)
    {
        return db.QueryAsync<SysCompany>(d => d.Code == db.User.CompNo);
    }

    //Organization
    internal static Task<bool> ExistsOrganizationAsync(Database db, SysOrganization entity)
    {
        return db.ExistsAsync<SysOrganization>(d => d.Id != entity.Id && d.CompNo == entity.CompNo && d.Code == entity.Code);
    }
}