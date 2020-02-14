using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Known.Core
{
    /// <summary>
    /// 平台服务类。
    /// </summary>
    public class PlatformService
    {
        private readonly IPlatformRepository repository;

        /// <summary>
        /// 初始化一个平台服务类的实例。
        /// </summary>
        public PlatformService()
        {
            repository = ObjectFactory.Create<IPlatformRepository>();
        }

        /// <summary>
        /// 获取指定应用程序的 API 接口根地址。
        /// </summary>
        /// <param name="apiId">应用程序 ID。</param>
        /// <returns>API 接口根地址。</returns>
        public string GetApiBaseUrl(string apiId)
        {
            return repository.GetApiBaseUrl(apiId);
        }

        /// <summary>
        /// 获取当前应用程序的系统代码列表。
        /// </summary>
        /// <returns>系统代码列表。</returns>
        public Dictionary<string, object> GetCodes()
        {
            return repository.GetCodes(AppInfo.Instance.Id);
        }

        /// <summary>
        /// 获取模块信息对象。
        /// </summary>
        /// <param name="id">模块 ID。</param>
        /// <returns>模块信息对象。</returns>
        public ModuleInfo GetModule(string id)
        {
            var module = repository.GetModule(id);
            if (module != null)
            {
                SetParentModule(module);
            }

            return module;
        }

        /// <summary>
        /// 获取当前应用程序的模块信息对象列表。
        /// </summary>
        /// <returns>模块信息对象列表。</returns>
        public List<ModuleInfo> GetModules()
        {
            var modules = repository.GetModules(AppInfo.Instance.Id);
            if (modules == null || modules.Count == 0)
                return null;

            return modules.OrderBy(m => m.ParentId)
                          .ThenBy(m => m.Sort)
                          .ToList();
        }

        /// <summary>
        /// 获取当前应用程序登录用户的模块信息对象列表。
        /// </summary>
        /// <param name="userName">登录用户名。</param>
        /// <returns>模块信息对象列表。</returns>
        public List<ModuleInfo> GetUserModules(string userName)
        {
            return AppInfo.Instance.Modules;
            //var modules = repository.GetUserModules(Setting.Instance.App.Id, userName);
            //if (modules == null || modules.Count == 0)
            //    return new List<ModuleInfo>();

            //return GetHierarchicalModules(modules);
        }

        /// <summary>
        /// 获取指定用户名的用户信息对象。
        /// </summary>
        /// <param name="userName">登录用户名。</param>
        /// <returns>用户信息对象。</returns>
        public UserInfo GetUser(string userName)
        {
            return repository.GetUser(userName);
        }

        internal Result<UserInfo> ValidateLogin(string token)
        {
            var user = UserCache.GetUserByToken(repository, token);
            if (user == null)
                return Result.Error<UserInfo>("用户不存在！");

            return Result.Success("登录成功！", user);
        }

        internal Result<UserInfo> ValidateLogin(string userName, string password)
        {
            var user = UserCache.GetUser(repository, userName);
            if (user == null)
                return Result.Error<UserInfo>("用户不存在！");

            if (user.Password != password)
                return Result.Error<UserInfo>("用户密码不正确！");

            return Result.Success("登录成功！", user);
        }

        /// <summary>
        /// 用户登录。
        /// </summary>
        /// <param name="userName">登录用户名。</param>
        /// <param name="password">登录密码。</param>
        /// <returns>用户登录验证结果。</returns>
        public Result<UserInfo> SignIn(string userName, string password)
        {
            var user = repository.GetUser(userName);
            if (user == null)
                return Result.Error<UserInfo>("用户不存在！");

            if (user.Password != password)
                return Result.Error<UserInfo>("用户密码不正确！");

            UserCache.RemoveUser(userName);
            UserCache.RemoveUserByToken(user.Token);

            user.Token = Utils.NewGuid;
            if (!user.FirstLoginTime.HasValue)
                user.FirstLoginTime = DateTime.Now;
            user.LastLoginTime = DateTime.Now;
            repository.SaveUser(user);

            return Result.Success("登录成功！", user);
        }

        /// <summary>
        /// 退出登录。
        /// </summary>
        /// <param name="userName">登录用户名。</param>
        /// <returns>退出登录结果。</returns>
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

        private void SetParentModule(ModuleInfo module)
        {
            if (module.ParentId == "0")
                return;

            module.Parent = repository.GetModule(module.ParentId);
            SetParentModule(module.Parent);
        }

        private List<ModuleInfo> GetHierarchicalModules(List<ModuleInfo> source)
        {
            var modules = new List<ModuleInfo>();
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

        private void SetModuleChildren(List<ModuleInfo> source, List<ModuleInfo> modules, ModuleInfo module)
        {
            var children = source.Where(m => m.ParentId == module.Id)
                                 .OrderBy(m => m.Sort)
                                 .ToList();
            if (children == null || children.Count == 0)
                return;

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
        private static readonly string appId = AppInfo.Instance.Id;

        public static void RemoveUser(string userName)
        {
            RemoveUserCache($"{appId}_{userName}");
        }

        public static void RemoveUserByToken(string token)
        {
            RemoveUserCache($"{appId}_{token}");
        }

        public static UserInfo GetUser(IPlatformRepository repository, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            return GetUserCache($"{appId}_{userName}", () =>
            {
                return repository.GetUser(userName);
            });
        }

        public static UserInfo GetUserByToken(IPlatformRepository repository, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            return GetUserCache($"{appId}_{token}", () =>
            {
                return repository.GetUserByToken(token);
            });
        }

        private static UserInfo GetUserCache(string key, Func<UserInfo> func)
        {
            if (!cached.ContainsKey(key))
            {
                lock (cached.SyncRoot)
                {
                    if (!cached.ContainsKey(key))
                    {
                        cached[key] = func();
                    }
                }
            }

            return (UserInfo)cached[key];
        }

        private static void RemoveUserCache(string key)
        {
            if (cached.ContainsKey(key))
            {
                lock (cached.SyncRoot)
                {
                    if (cached.ContainsKey(key))
                    {
                        cached.Remove(key);
                    }
                }
            }
        }
    }
}
