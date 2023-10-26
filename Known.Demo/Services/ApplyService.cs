using Known.Demo.Repositories;
using Known.Demo.WorkFlows;

namespace Known.Demo.Services;

public class ApplyService : BaseService
{
    //Apply
    public Task<PagingResult<TbApply>> QueryApplysAsync(PagingCriteria criteria)
    {
        return ApplyRepository.QueryApplysAsync(Database, criteria);
    }

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

    public Task<TbApply> GetApplyAsync(string id)
    {
        return Database.QueryByIdAsync<TbApply>(id);
    }

    public async Task<Result> DeleteApplysAsync(List<TbApply> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        if (models.Exists(m => m.BizStatus != FlowStatus.Save))
            return Result.Error("只能删除暂存状态的记录！");

        var oldFiles = new List<string>();
        var result = await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                //删除流程
                await PlatformHelper.DeleteFlowAsync(db, item.Id);
                //删除附件
                await PlatformHelper.DeleteFilesAsync(db, item.Id, oldFiles);
                //删除实体
                await db.DeleteAsync(item);
            }
        });
        //如果事务执行成功，删除实际附件
        if (result.IsValid)
            AttachFile.DeleteFiles(oldFiles);
        return result;
    }

    public async Task<Result> SaveApplyAsync(UploadFormInfo info)
    {
        var entity = await Database.QueryByIdAsync<TbApply>((string)info.Model.Id);
        entity ??= new TbApply();
        entity.FillModel(info.Model);
        var vr = entity.Validate();
        if (!vr.IsValid)
            return vr;

        var user = CurrentUser;
        //保存实际附件
        var bizType = "ApplyFiles";
        var bizFiles = GetAttachFiles(info, user, nameof(TbApply.BizFile), bizType);
        return await Database.TransactionAsync(Language.Save, async db =>
        {
            if (entity.IsNew)
            {
                entity.BizNo = await GetMaxBizNoAsync(db, entity.BizType);
                //创建流程
                await PlatformHelper.CreateFlowAsync(db, ApplyFlow.GetBizInfo(entity));
            }
            //保存附件
            await PlatformHelper.AddFilesAsync(db, bizFiles, entity.Id, bizType);
            entity.BizFile = $"{entity.Id}_{bizType}";
            await db.SaveAsync(entity);
        }, entity);
    }

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