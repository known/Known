namespace Sample.Core.Repositories;

class ApplyRepository
{
    //Apply
    internal static PagingResult<TbApply> QueryApplys(Database db, PagingCriteria criteria)
    {
        var sql = "select * from TbApply where CompNo=@CompNo";
        return db.QueryPage<TbApply>(sql, criteria);
    }

    internal static string GetMaxBizNo(Database db, string prefix)
    {
        var sql = $"select max(BizNo) from TbApply where CompNo=@CompNo and BizNo like '{prefix}%'";
        return db.Scalar<string>(sql, new { db.User.CompNo });
    }
}