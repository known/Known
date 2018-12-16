using System;
using System.Collections.Generic;
using System.Linq;
using Known.Platform.Repositories;

namespace Known.Platform.Services
{
    public class UserService : PlatformService<IUserRepository>
    {
        public UserService(Context context) : base(context)
        {
        }

        public Result<User> SignIn(string userName, string password)
        {
            var result = ValidateLogin(userName, password);
            if (!result.IsValid)
                return Result.Error<User>(result.Message);

            var user = result.Data;
            user.Token = Utils.NewGuid;
            if (!user.FirstLoginTime.HasValue)
                user.FirstLoginTime = DateTime.Now;
            user.LastLoginTime = DateTime.Now;
            Repository.Save(user);

            return Result.Success("登录成功！", user);
        }

        public Result SignOut(string userName)
        {
            var user = GetUser(userName);
            if (user == null)
                return Result.Error("用户名不存在！");

            user.Token = string.Empty;
            Repository.Save(user);

            return Result.Success("注销成功！");
        }

        public Result<User> ValidateLogin(string userName, string password)
        {
            var user = GetUser(userName);
            if (user == null)
                return Result.Error<User>("用户名不存在！");

            if (user.Password != password)
                return Result.Error<User>("用户密码不正确！");

            return Result.Success("登录成功！", user);
        }

        public User GetUser(string userName)
        {
            return Repository.GetUser(userName);
        }

        #region GetUserModules
        public List<Module> GetUserModules()
        {
            //var path = Path.Combine(HttpRuntime.AppDomainAppPath, "menu.json");
            //var json = File.ReadAllText(path);
            //return ApiResult.Success(json.FromJson<object>());

            var modules = Repository.GetUserModules(Context.UserName);
            if (modules == null || modules.Count == 0)
                return new List<Module>();

            return GetHierarchicalModules(modules);
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
        #endregion
    }
}
