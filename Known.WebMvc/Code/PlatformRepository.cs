using System.Collections.Generic;
using Known.Platform;

namespace Known.WebMvc
{
    public class PlatformRepository : IPlatformRepository
    {
        public string GetApiBaseUrl(string apiId)
        {
            var apiUrl = string.Empty;
            if (apiId == "plt")
                apiUrl = "http://localhost:8089";

            return apiUrl;
        }

        public Dictionary<string, object> GetCodes(string appId)
        {
            return new Dictionary<string, object>();
        }

        public Module GetModule(string id)
        {
            return new Module { Id = id };
        }

        public List<Module> GetModules(string appId)
        {
            return new List<Module>();
        }

        public List<Module> GetUserModules(string userName)
        {
            return new List<Module>();
        }

        public User GetUser(string userName)
        {
            return new User { UserName = userName };
        }

        public void SaveUser(User user)
        {
        }
    }
}