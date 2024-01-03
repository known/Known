using Known.Demo.Entities;
using Known.Demo.Repositories;
using Known.Demo.WorkFlows;
using Known.Extensions;
using Known.WorkFlows;

namespace Known.Demo.Services;

//业务申请逻辑服务
class ApplyService : ServiceBase
{
    //Apply
    //列表分页查询
    public Task<PagingResult<TbApply>> QueryApplysAsync(FlowPageType type, PagingCriteria criteria)
    {
        return ApplyRepository.QueryApplysAsync(Database, type, criteria);
    }

    //获取默认业务申请实体
    public async Task<TbApply> GetDefaultApplyAsync(ApplyType bizType)
    {
        return new TbApply
        {
            BizType = bizType,
            BizNo = await GetMaxBizNoAsync(Database, bizType),
            BizStatus = FlowStatus.Save,
            ApplyBy = CurrentUser.Name,
            ApplyTime = DateTime.Now
        };
    }

    //删除业务申请
    public async Task<Result> DeleteApplysAsync(List<TbApply> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error("请至少选择一条记录进行操作！");

        if (models.Exists(m => m.BizStatus != FlowStatus.Save))
            return Result.Error("只能删除暂存状态的记录！");

        var oldFiles = new List<string>();
        var result = await Database.TransactionAsync("删除", async db =>
        {
            foreach (var item in models)
            {
                //删除流程
                await Platform.DeleteFlowAsync(db, item.Id);
                //删除附件
                await Platform.DeleteFilesAsync(db, item.Id, oldFiles);
                //删除实体
                await db.DeleteAsync(item);
            }
        });
        //如果事务执行成功，删除实际附件
        if (result.IsValid)
            Platform.DeleteFiles(oldFiles);
        return result;
    }

    //保存业务申请
    public async Task<Result> SaveApplyAsync(UploadInfo<TbApply> info)
    {
        var entity = info.Model;
        var vr = entity.Validate(Context);
        if (!vr.IsValid)
            return vr;

        var user = CurrentUser;
        //保存实际附件
        var bizType = "ApplyFiles";
        var bizFiles = info.Files.GetAttachFiles(user, nameof(TbApply.BizFile), bizType);
        return await Database.TransactionAsync(Language.Save, async db =>
        {
            if (entity.IsNew)
            {
                entity.BizNo = await GetMaxBizNoAsync(db, entity.BizType);
                //创建流程
                await Platform.CreateFlowAsync(db, ApplyFlow.GetBizInfo(entity));
            }
            //添加附件
            await Platform.AddFilesAsync(db, bizFiles, entity.Id, bizType);
            entity.BizFile = $"{entity.Id}_{bizType}";
            //保存实体
            await db.SaveAsync(entity);
        }, entity);
    }

    //获取最大业务申请单号
    private static async Task<string> GetMaxBizNoAsync(Database db, ApplyType bizType)
    {
        var prefix = "T";
        prefix += $"{DateTime.Now:yyyy}";
        var maxNo = await ApplyRepository.GetMaxBizNoAsync(db, prefix);
        if (string.IsNullOrWhiteSpace(maxNo))
            maxNo = $"{prefix}00000";
        return GetMaxFormNo(prefix, maxNo);
    }
}