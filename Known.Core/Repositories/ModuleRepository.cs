using Known.Data;

namespace Known.Platform.Repositories
{
    public interface IModuleRepository : IRepository
    {
    }

    class ModuleRepository : DbRepository, IModuleRepository
    {
        public ModuleRepository(Database database) : base(database)
        {
        }
    }
}
