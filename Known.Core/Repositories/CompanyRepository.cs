namespace Known.Core.Repositories;

class CompanyRepository
{
    internal static PagingResult<SysCompany> QueryCompanys(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysCompany where AppId=@AppId and CompNo=@CompNo";
        return db.QueryPage<SysCompany>(sql, criteria);
    }

    internal static SysCompany GetCompany(Database db, string code)
    {
        var sql = "select * from SysCompany where Code=@code";
        return db.Query<SysCompany>(sql, new { code });
    }
}