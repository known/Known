using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Known.Platform
{
    public class PlatformService
    {
        private readonly IPlatformRepository repository;

        public PlatformService()
        {
            repository = ObjectFactory.Create<IPlatformRepository>();
        }

        public string GetApiBaseUrl(string apiId)
        {
            return repository.GetApiBaseUrl(apiId);
        }

        public Dictionary<string, object> GetCodes()
        {
            return repository.GetCodes(Setting.Instance.AppId);
        }

        public Module GetModule(string id)
        {
            var module = repository.GetModule(id);
            if (module != null)
            {
                SetParentModule(module);
            }

            return module;
        }

        public List<Module> GetModules()
        {
            var modules = repository.GetModules(Setting.Instance.AppId);
            if (modules == null || modules.Count == 0)
                return null;

            return modules.OrderBy(m => m.ParentId)
                          .ThenBy(m => m.Sort)
                          .ToList();
        }

        public List<Module> GetUserModules(string userName)
        {
            var modules = repository.GetUserModules(Setting.Instance.AppId, userName);
            if (modules == null || modules.Count == 0)
                return new List<Module>();

            return GetHierarchicalModules(modules);
        }

        public User GetUser(string userName)
        {
            return repository.GetUser(userName);
        }

        public Result<User> ValidateLogin(string token)
        {
            var user = UserCache.GetUserByToken(repository, token);
            if (user == null)
                return Result.Error<User>("用户不存在！");

            return Result.Success("登录成功！", user);
        }

        public Result<User> ValidateLogin(string userName, string password)
        {
            var user = UserCache.GetUser(repository, userName);
            if (user == null)
                return Result.Error<User>("用户不存在！");

            if (user.Password != password)
                return Result.Error<User>("用户密码不正确！");

            return Result.Success("登录成功！", user);
        }

        public Result<User> SignIn(string userName, string password)
        {
            var user = repository.GetUser(userName);
            if (user == null)
                return Result.Error<User>("用户不存在！");

            if (user.Password != password)
                return Result.Error<User>("用户密码不正确！");

            UserCache.RemoveUser(userName);
            UserCache.RemoveUserByToken(user.Token);

            user.Token = Utils.NewGuid;
            if (!user.FirstLoginTime.HasValue)
                user.FirstLoginTime = DateTime.Now;
            user.LastLoginTime = DateTime.Now;
            repository.SaveUser(user);

            return Result.Success("登录成功！", user);
        }

        public Result SignOut(string userName)
        {
            var user = GetUser(userName);
            if (user == null)
                return Result.Error("用户不存在！");

            UserCache.RemoveUser(userName);
            UserCache.RemoveUserByToken(user.Token);

            user.Token = string.Empty;
            repository.SaveUser(user);

            return Result.Success("注销成功！");
        }

        private void SetParentModule(Module module)
        {
            if (module.ParentId == "0")
                return;

            module.Parent = repository.GetModule(module.ParentId);
            SetParentModule(module.Parent);
        }

        private List<Module> GetHierarchicalModules(List<Module> source)
        {
            var modules = new List<Module>();
            var topModules = source.Where(m => m.ParentId == "0")
                                   .OrderBy(m => m.Sort)
                                   .ToList();
            foreach (var item in topModules)
            {
                modules.Add(item);
                SetModuleChildren(source, modules, item);
            }

            return modules;
        }

        private void SetModuleChildren(List<Module> source, List<Module> modules, Module module)
        {
            var children = source.Where(m => m.ParentId == module.Id)
                                 .OrderBy(m => m.Sort)
                                 .ToList();
            if (children == null || children.Count == 0)
                return;

            if (module.Children == null)
                module.Children = new List<Module>();

            foreach (var item in children)
            {
                item.Parent = module;
                module.Children.Add(item);
                SetModuleChildren(source, modules, item);
            }
        }
    }

    class UserCache
    {
        private static readonly Hashtable cached = new Hashtable();
        private static readonly string appId = Setting.Instance.AppId;

        public static void RemoveUser(string userName)
        {
            var key = $"{appId}_{userName}";
            if (cached.ContainsKey(key))
            {
                cached.Remove(key);
            }
        }

        public static void RemoveUserByToken(string token)
        {
            var key = $"{appId}_{token}";
            if (cached.ContainsKey(key))
            {
                cached.Remove(key);
            }
        }

        public static User GetUser(IPlatformRepository repository, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            var key = $"{appId}_{userName}";
            if (!cached.ContainsKey(key))
            {
                lock (cached.SyncRoot)
                {
                    if (!cached.ContainsKey(key))
                    {
                        cached[key] = repository.GetUser(userName);
                    }
                }
            }

            return (User)cached[key];
        }

        public static User GetUserByToken(IPlatformRepository repository, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var key = $"{appId}_{token}";
            if (!cached.ContainsKey(key))
            {
                lock (cached.SyncRoot)
                {
                    if (!cached.ContainsKey(key))
                    {
                        cached[key] = repository.GetUserByToken(token);
                    }
                }
            }

            return (User)cached[key];
        }
    }
}
