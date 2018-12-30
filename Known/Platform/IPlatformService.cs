using System.Collections.Generic;

namespace Known.Platform
{
    public interface IPlatformService
    {
        string GetApiUrl(string apiId);
        Dictionary<string, object> GetCodes();
        Module GetModule(string id);
        List<Module> GetModules();
        List<Module> GetUserModules(string userName);
        User GetUser(string userName);
        Result<User> ValidateLogin(string userName, string password);
        Result<User> SignIn(string userName, string password);
    }
}
