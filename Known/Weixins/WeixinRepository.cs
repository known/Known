namespace Known.Weixins;

class WeixinRepository
{
    internal static Task<SysWeixin> GetWeixinByUserIdAsync(Database db, string userId)
    {
        var sql = "select * from SysWeixin where UserId=@userId";
        return db.QueryAsync<SysWeixin>(sql, new { userId });
    }

    internal static Task<SysWeixin> GetWeixinByOpenIdAsync(Database db, string openId)
    {
        var sql = "select * from SysWeixin where OpenId=@openId";
        return db.QueryAsync<SysWeixin>(sql, new { openId });
    }
}