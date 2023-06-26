namespace Known.Core.Repositories;

class SystemRepository
{
    //Tenant
    internal static PagingResult<SysTenant> QueryTenants(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysTenant where AppId=@AppId and CompNo=@CompNo";
        return db.QueryPage<SysTenant>(sql, criteria);
    }

    internal static bool ExistsTenant(Database db, string id, string code)
    {
        var sql = "select count(*) from SysTenant where Id<>@id and Code=@code";
        return db.Scalar<int>(sql, new { id, code }) > 0;
    }
}