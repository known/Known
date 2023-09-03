namespace Sample.Core.Services;

class ApplyService : ServiceBase
{
    internal ApplyService(Context context) : base(context) { }

    //Apply
    internal PagingResult<TbApply> QueryApplys(PagingCriteria criteria)
    {
        return ApplyRepository.QueryApplys(Database, criteria);
    }

    internal TbApply GetDefaultApply(ApplyType bizType)
    {
        return new TbApply
        {
            BizType = bizType,
            BizNo = GetMaxBizNo(Database, bizType),
            BizStatus = FlowStatus.Save,
            ApplyBy = CurrentUser.Name,
            ApplyTime = DateTime.Now
        };
    }

    internal TbApply GetApply(string id)
    {
        return Database.QueryById<TbApply>(id);
    }

    internal Result DeleteApplys(List<TbApply> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        if (models.Exists(m => m.BizStatus != FlowStatus.Save))
            return Result.Error("只能删除暂存状态的记录！");

        var oldFiles = new List<string>();
        var result = Database.Transaction(Language.Delete, db =>
        {
            foreach (var item in models)
            {
                //删除流程
                PlatformHelper.DeleteFlow(db, item.Id);
                //删除附件
                PlatformHelper.DeleteFiles(db, item.Id, oldFiles);
                //删除实体
                db.Delete(item);
            }
        });
        //如果事务执行成功，删除实际附件
        if (result.IsValid)
            AttachFile.DeleteFiles(oldFiles);
        return result;
    }

    internal Result SaveApply(UploadFormInfo info)
    {
        var entity = Database.QueryById<TbApply>((string)info.Model.Id);
        entity ??= new TbApply();
        entity.FillModel(info.Model);
        var vr = entity.Validate();
        if (!vr.IsValid)
            return vr;

        var user = CurrentUser;
        //保存实际附件
        var bizType = "ApplyFiles";
        var bizFiles = GetAttachFiles(info, user, nameof(TbApply.BizFile), bizType);
        return Database.Transaction(Language.Save, db =>
        {
            if (entity.IsNew)
            {
                entity.BizNo = GetMaxBizNo(db, entity.BizType);
                //创建流程
                PlatformHelper.CreateFlow(db, ApplyFlow.GetBizInfo(entity));
            }
            //保存附件
            PlatformHelper.AddFiles(db, bizFiles, entity.Id, bizType);
            entity.BizFile = $"{entity.Id}_{bizType}";
            db.Save(entity);
        }, entity);
    }

    private static string GetMaxBizNo(Database db, ApplyType bizType)
    {
        var prefix = "T";
        prefix += $"{DateTime.Now:yyyy}";
        var maxNo = ApplyRepository.GetMaxBizNo(db, prefix);
        if (string.IsNullOrWhiteSpace(maxNo))
            maxNo = $"{prefix}00000";
        return GetMaxFormNo(prefix, maxNo);
    }
}