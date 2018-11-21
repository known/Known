using Known.Data;

namespace Known.Platform.Repositories
{
    public interface IModuleRepository : IRepository
    {
    }

    internal class ModuleRepository : DbRepository, IModuleRepository
    {
    }
}
