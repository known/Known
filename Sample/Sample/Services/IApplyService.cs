namespace Sample.Services;

public interface IApplyService : IService
{
    Task<PagingResult<TbApply>> QueryApplysAsync(PagingCriteria criteria);
    Task<TbApply> GetDefaultApplyAsync(string bizType);
    Task<Result> DeleteApplysAsync(List<TbApply> models);
    Task<Result> SaveApplyAsync(UploadInfo<TbApply> info);

    Task<PagingResult<TbApplyList>> QueryApplyListsAsync(PagingCriteria criteria);
    Task<Result> DeleteApplyListsAsync(List<TbApplyList> models);
    Task<Result> SaveApplyListAsync(TbApplyList model);
}

//业务申请逻辑服务
class ApplyService(Context context) : ServiceBase(context), IApplyService
{
    #region Apply
    //列表分页查询
    public Task<PagingResult<TbApply>> QueryApplysAsync(PagingCriteria criteria)
    {
        return ApplyRepository.QueryApplysAsync(Database, criteria);
    }

    //获取默认业务申请实体
    public async Task<TbApply> GetDefaultApplyAsync(string bizType)
    {
        return new TbApply
        {
            BizType = bizType.ToString(),
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
            return Result.Error(Language.SelectOneAtLeast);

        if (models.Exists(m => m.BizStatus != FlowStatus.Save))
            return Result.Error(Language["Tip.FlowDeleteSave"]);

        var oldFiles = new List<string>();
        var result = await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                //删除流程
                await db.DeleteFlowAsync(item.Id);
                //删除附件
                await db.DeleteFilesAsync(item.Id, oldFiles);
                //删除实体
                await db.DeleteAsync(item);
            }
        });
        //如果事务执行成功，删除实际附件
        if (result.IsValid)
            AttachFile.DeleteFiles(oldFiles);
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
                await db.CreateFlowAsync(ApplyFlow.GetBizInfo(entity));
            }
            //添加附件
            await db.AddFilesAsync(bizFiles, entity.Id, bizType);
            entity.BizFile = $"{entity.Id}_{bizType}";
            //保存实体
            await db.SaveAsync(entity);
        }, entity);
    }

    //获取最大业务申请单号
    private static async Task<string> GetMaxBizNoAsync(Database db, string bizType)
    {
        var prefix = "T";
        prefix += $"{DateTime.Now:yyyy}";
        var maxNo = await ApplyRepository.GetMaxBizNoAsync(db, prefix);
        if (string.IsNullOrWhiteSpace(maxNo))
            maxNo = $"{prefix}00000";
        return Utils.GetMaxFormNo(prefix, maxNo);
    }
    #endregion

    #region ApplyList
    public Task<PagingResult<TbApplyList>> QueryApplyListsAsync(PagingCriteria criteria)
    {
        return Database.QueryPageAsync<TbApplyList>(criteria);
    }

    public async Task<Result> DeleteApplyListsAsync(List<TbApplyList> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await db.DeleteAsync(item);
            }
        });
    }

    public async Task<Result> SaveApplyListAsync(TbApplyList model)
    {
        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
        }, model);
    }
    #endregion
}