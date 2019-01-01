using System.Collections.Generic;

namespace Known.Platform
{
    public interface IPlatformRepository
    {
        string GetApiBaseUrl(string apiId);
        Dictionary<string, object> GetCodes();
        Module GetModule(string id);
        List<Module> GetModules();
        List<Module> GetUserModules(string userName);
        User GetUser(string userName);
        void SaveUser(User user);
    }
}
