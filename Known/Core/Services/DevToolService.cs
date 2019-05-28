namespace Known.Core.Services
{
    public class DevToolService : ServiceBase
    {
        public DevToolService(Context context) : base(context)
        {
        }

        #region DevDatabase
        public PagingResult QueryDatas(PagingCriteria criteria)
        {
            var sql = criteria.Parameter.querySql.ToString();
            return Context.Database.QueryPageTable(sql, criteria);
        }
        #endregion
    }
}
