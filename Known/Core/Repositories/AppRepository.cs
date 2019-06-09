using Known.Data;

namespace Known.Core
{
    /// <summary>
    /// 应用程序数据仓库接口。
    /// </summary>
    public interface IAppRepository : IRepository
    {

    }

    internal class AppRepository : DbRepository, IAppRepository
    {
        public AppRepository(Database database) : base(database)
        {
        }
    }
}
