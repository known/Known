namespace Known.Winxins;

class WeixinRepository
{
    internal static Task<SysWinxin> GetWinxinByUserIdAsync(Database db, string userId)
    {
        var sql = "select * from SysWinxin where UserId=@userId";
        return db.QueryAsync<SysWinxin>(sql, new { userId });
    }

    internal static Task<SysWinxin> GetWinxinByOpenIdAsync(Database db, string openId)
    {
        var sql = "select * from SysWinxin where OpenId=@openId";
        return db.QueryAsync<SysWinxin>(sql, new { openId });
    }
}