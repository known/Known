namespace Sample.Services;

public interface IApplyService : IService
{
    Task<PagingResult<TbApply>> QueryApplysAsync(PagingCriteria criteria);
    Task<TbApply> GetDefaultApplyAsync(ApplyType bizType);
    Task<Result> DeleteApplysAsync(List<TbApply> models);
    Task<Result> SaveApplyAsync(UploadInfo<TbApply> info);
}