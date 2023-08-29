using Known.Extensions;

namespace Sample.Core.Repositories;

class ApplyRepository
{
    //Apply
    internal static PagingResult<TbApply> QueryApplys(Database db, PagingCriteria criteria)
    {
        var sql = @"select a.*,b.BizStatus,b.CurrBy,b.ApplyBy,b.ApplyTime,b.VerifyBy,b.VerifyTime,b.VerifyNote 
from TbApply a,SysFlow b 
where a.Id=b.BizId and a.CompNo=@CompNo";

        var type = criteria.Parameters.GetValue<PageType>(nameof(PageType));
        switch (type)
        {
            case PageType.Apply:
                sql += $" and b.BizStatus<>'{FlowStatus.VerifyPass}' and (b.CreateBy='{db.UserName}' or b.ApplyBy='{db.UserName}' or (b.BizStatus='{FlowStatus.Verifing}' and b.ApplyBy='{db.UserName}'))";
                break;
            case PageType.Verify:
                sql += $" and b.BizStatus='{FlowStatus.Verifing}' and b.CurrBy='{db.UserName}'";
                break;
            case PageType.Query:
                sql += $" and b.BizStatus='{FlowStatus.VerifyPass}'";
                break;
            default:
                break;
        }

        return db.QueryPage<TbApply>(sql, criteria);
    }

    internal static string GetMaxBizNo(Database db, string prefix)
    {
        var sql = $"select max(BizNo) from TbApply where CompNo=@CompNo and BizNo like '{prefix}%'";
        return db.Scalar<string>(sql, new { db.User.CompNo });
    }
}