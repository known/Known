namespace Sample.Web.Repositories;

//业务申请数据访问
class ApplyRepository
{
    //Apply
    //分页查询
    internal static Task<PagingResult<TbApply>> QueryApplysAsync(Database db, PagingCriteria criteria)
    {
        var sql = @"select a.*,b.BizStatus,b.CurrStep,b.CurrBy,b.ApplyBy,b.ApplyTime,b.VerifyBy,b.VerifyTime,b.VerifyNote 
from TbApply a,SysFlow b 
where a.Id=b.BizId and a.CompNo=@CompNo";

        var userName = db.User.UserName;
        var type = criteria.GetParameter<FlowPageType>("Type");
        switch (type)
        {
            case FlowPageType.Apply:
                sql += $" and b.BizStatus<>'{FlowStatus.VerifyPass}' and (b.CreateBy='{userName}' or b.ApplyBy='{userName}' or (b.BizStatus='{FlowStatus.Verifing}' and b.ApplyBy='{userName}'))";
                break;
            case FlowPageType.Verify:
                sql += $" and b.BizStatus='{FlowStatus.Verifing}' and b.CurrBy='{userName}'";
                break;
            case FlowPageType.Query:
                sql += $" and b.BizStatus='{FlowStatus.VerifyPass}'";
                break;
            default:
                break;
        }
        criteria.StatisColumns = [
            new StatisColumnInfo { Id = nameof(TbApply.Id), Function = "count" },
            new StatisColumnInfo { 
                Id = "VerifingCount", 
                Expression = $"sum(case when BizStatus='{FlowStatus.Verifing}' then 1 else 0 end)" 
            },
        ];
        return db.QueryPageAsync<TbApply>(sql, criteria);
    }

    //获取最大业务申请单号
    internal static Task<string> GetMaxBizNoAsync(Database db, string prefix)
    {
        var sql = $"select max(BizNo) from TbApply where CompNo=@CompNo and BizNo like '{prefix}%'";
        return db.ScalarAsync<string>(sql, new { db.User.CompNo });
    }
}