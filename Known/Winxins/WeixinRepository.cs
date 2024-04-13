namespace Known.Winxins;

class WeixinRepository
{
    internal static Task<SysWinxin> GetWinxinByUserIdAsync(Database db, string userId)
    {
        var sql = "select * from SysWinxin where UserId=@userId";
        return db.QueryAsync<SysWinxin>(sql, new { userId });
    }
}