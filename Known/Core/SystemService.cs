namespace Known.Core
{
    public partial class SystemService : ServiceBase, IService
    {
        private static ISystemRepository Repository => Container.Resolve<ISystemRepository>(new SystemRepository());
        internal const string DevId = "DEV";
    }

    public partial interface ISystemRepository
    {
    }

    partial class SystemRepository : ISystemRepository
    {
    }
}