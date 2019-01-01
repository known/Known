using System.Collections.Generic;

namespace Known.Platform
{
    abstract class PlatformHelper
    {
        public abstract string GetApiBaseUrl(string apiId);
        public abstract Dictionary<string, object> GetCodes();
        public abstract Module GetModule(string id);
        public abstract List<Module> GetModules();
        public abstract List<Module> GetUserModules(string userName);
        public abstract User GetUser(string userName);
        public abstract void SaveUser(User user);
    }
}