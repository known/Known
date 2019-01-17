using System.Collections.Generic;

namespace Known.Platform
{
    class DbPlatformHelper : PlatformHelper
    {
        private readonly IPlatformRepository repository;

        public DbPlatformHelper(IPlatformRepository repository)
        {
            if (repository == null)
            {
                repository = Container.Resolve<IPlatformRepository>();
            }

            this.repository = repository;
        }

        public override string GetApiBaseUrl(string apiId)
        {
            if (string.IsNullOrWhiteSpace(apiId))
                return null;

            return repository.GetApiBaseUrl(apiId);
        }

        public override Dictionary<string, object> GetCodes(string appId)
        {
            return repository.GetCodes(appId);
        }

        public override Module GetModule(string id)
        {
            return repository.GetModule(id);
        }

        public override List<Module> GetModules(string appId)
        {
            return repository.GetModules(appId);
        }

        public override List<Module> GetUserModules(string userName)
        {
            return repository.GetUserModules(userName);
        }

        public override User GetUser(string userName)
        {
            return repository.GetUser(userName);
        }

        public override void SaveUser(User user)
        {
            repository.SaveUser(user);
        }
    }
}
