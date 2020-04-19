using System;
using System.Collections.Generic;
using System.Web;
using Known.Core.Entities;
using Known.Web;

namespace Known.Core
{
    public class PlatformService : ServiceBase
    {
        private IPlatformRepository Repository
        {
            get { return Container.Resolve<IPlatformRepository>(); }
        }

        #region Login
        public Result SignIn(string userName, string password)
        {
            var entity = Repository.GetUser(Database, userName);
            if (entity == null)
                return Result.Error("用户名不存在！");

            var pwd = Utils.ToMd5(password);
            if (entity.Password != pwd)
                return Result.Error("密码不正确！");

            var ip = Utils.GetIPAddress(HttpContext.Current.Request);
            if (!entity.FirstLoginTime.HasValue)
            {
                entity.FirstLoginTime = DateTime.Now;
                entity.FirstLoginIP = ip;
            }
            entity.LastLoginTime = DateTime.Now;
            entity.LastLoginIP = ip;

            var user = Utils.MapTo<UserInfo>(entity);
            SessionHelper.SetUser(user);
            Database.Save(entity);
            return Result.Success("登录成功！", user);
        }
        #endregion

        #region Index
        public void SignOut(string userName)
        {
        }

        public List<MenuInfo> GetUserMenus(string userName)
        {
            var menus = Repository.GetUserMenus(Database, userName);
            if (userName == "System")
            {
                var dev = Utils.GetResource(GetType().Assembly, "DevMenu");
                menus.InsertRange(0, Utils.FromJson<List<MenuInfo>>(dev));
            }

            return menus;
        }
        #endregion

        #region Welcome
        public PagingResult<object> GetTodoLists(PagingCriteria criteria)
        {
            return new PagingResult<object>
            {
                TotalCount = 33,
                PageData = new List<object>
                {
                    new {Id="1",Name="请假审批",Qty=1,CreateTime=DateTime.Now},
                    new {Id="2",Name="费用报销",Qty=2,CreateTime=DateTime.Now},
                    new {Id="3",Name="出差审批",Qty=3,CreateTime=DateTime.Now}
                }
            };
        }

        public PagingResult<object> GetCompanyNews(PagingCriteria criteria)
        {
            return new PagingResult<object>
            {
                TotalCount = 55,
                PageData = new List<object>
                {
                    new {Id="1",Title="公司新系统上线",CreateBy="管理员",CreateTime=DateTime.Now},
                    new {Id="2",Title="关于放假通知",CreateBy="张三",CreateTime=DateTime.Now},
                    new {Id="3",Title="关于员工福利通知",CreateBy="李四",CreateTime=DateTime.Now}
                }
            };
        }

        internal PagingResult<object> GetShortCuts()
        {
            return null;
        }
        #endregion

        #region UserInfo
        public UserInfo GetUserInfo(string userName)
        {
            var user = SessionHelper.GetUser();
            if (user == null && !string.IsNullOrWhiteSpace(userName))
            {
                var entity = Repository.GetUser(Database, userName);
                user = Utils.MapTo<UserInfo>(entity);
                SessionHelper.SetUser(user);
            }
            return user;
        }

        public Result SaveUserInfo(dynamic model)
        {
            var entity = Database.QueryById<SysUser>((string)model.Id);
            if (entity == null)
                return Result.Error("当前用户不存在！");

            entity.FillModel(model);
            var vr = entity.Validate();
            if (!vr.IsValid)
                return Result.Error(vr.Message);

            Database.Save(entity);
            return Result.Success("保存成功！", entity.Id);
        }

        public Result UpdatePassword(UserInfo user, string oldPassword, string password, string repassword)
        {
            if (user == null)
                return Result.Error("当前用户未登录！");

            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(oldPassword))
                errors.Add("当前密码不能为空！");
            if (string.IsNullOrWhiteSpace(password))
                errors.Add("新密码不能为空！");
            if (string.IsNullOrWhiteSpace(repassword))
                errors.Add("确认新密码不能为空！");
            if (password != repassword)
                errors.Add("两次密码输入不一致！");

            if (errors.Count > 0)
                return Result.Error(string.Join(Environment.NewLine, errors));

            var entity = Database.QueryById<SysUser>(user.Id);
            if (entity == null)
                return Result.Error("当前用户不存在！");

            var pwd = Utils.ToMd5(oldPassword);
            if (entity.Password != pwd)
                return Result.Error("当前密码不正确！");

            entity.Password= Utils.ToMd5(password);
            Database.Save(entity);
            return Result.Success("修改成功！", entity.Id);
        }
        #endregion
    }
}
