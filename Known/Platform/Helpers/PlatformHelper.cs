using System.Collections.Generic;

namespace Known.Platform
{
    abstract class PlatformHelper
    {
        public abstract string GetApiBaseUrl(string apiId);
        public abstract Dictionary<string, object> GetCodes(string appId);
        public abstract Module GetModule(string id);
        public abstract List<Module> GetModules(string appId);
        public abstract List<Module> GetUserModules(string appId, string userName);
        public abstract User GetUser(string userName);
        public abstract User GetUserByToken(string token);
        public abstract void SaveUser(User user);
    }
}