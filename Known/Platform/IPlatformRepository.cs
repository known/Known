using System.Collections.Generic;

namespace Known.Platform
{
    public interface IPlatformRepository
    {
        string GetApiBaseUrl(string apiId);
        Dictionary<string, object> GetCodes(string appId);
        Module GetModule(string id);
        List<Module> GetModules(string appId);
        List<Module> GetUserModules(string userName);
        User GetUser(string userName);
        void SaveUser(User user);
    }
}
