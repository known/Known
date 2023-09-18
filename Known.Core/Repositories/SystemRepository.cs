namespace Known.Core.Repositories;

class SystemRepository
{
    //Tenant
    internal static PagingResult<SysTenant> QueryTenants(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysTenant where AppId=@AppId";
        if (db.User.IsOperation)
        {
            sql += " and OperateBy=@OperateBy";
            criteria.SetQuery("OperateBy", QueryType.Equal, $"{db.User.UserName}-{db.User.Name}");
        }
        else
        {
            sql += " and CompNo=@CompNo";
        }
        return db.QueryPage<SysTenant>(sql, criteria);
    }

    internal static SysTenant GetTenant(Database db, string code)
    {
        var sql = "select * from SysTenant where Code=@code";
        return db.Query<SysTenant>(sql, new { code });
    }

    internal static bool ExistsTenant(Database db, string id, string code)
    {
        var sql = "select count(*) from SysTenant where Id<>@id and Code=@code";
        return db.Scalar<int>(sql, new { id, code }) > 0;
    }
}