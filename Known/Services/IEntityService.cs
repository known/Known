namespace Known.Services
{
    public interface IEntityService<TEntity> : IService
    {
        Task<PagingResult<TEntity>> QueryAsync(PagingCriteria criteria);
        Task<TEntity> GetAsync(string id);
        Task<Result> DeleteAsync(List<TEntity> models);
        Task<Result> SaveAsync(TEntity model);
    }
}
