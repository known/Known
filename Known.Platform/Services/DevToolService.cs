namespace Known.Platform.Services
{
    public class DevToolService : ServiceBase
    {
        public DevToolService(Context context) : base(context)
        {
        }

        public PagingResult QueryDatas(PagingCriteria criteria)
        {
            var sql = criteria.Parameter.querySql.ToString();
            criteria.Parameter = null;
            return Context.Database.QueryPageTable(sql, criteria);
        }
    }
}
