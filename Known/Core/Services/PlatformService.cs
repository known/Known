using System;
using System.Collections.Generic;
using System.Web;
using Known.Core.Datas;

namespace Known.Core.Services
{
    public class PlatformService : ServiceBase
    {
        private IPlatformRepository Repository
        {
            get { return Container.Resolve<IPlatformRepository>(); }
        }

        #region Login
        internal Result SignIn(string userName, string password)
        {
            var user = Repository.GetUser(Database, userName);
            if (user == null)
                return Result.Error("用户名不存在！");

            var pwd = Utils.ToMd5(password);
            if (user.Password != pwd)
                return Result.Error("密码不正确！");

            var ip = Utils.GetIPAddress(HttpContext.Current.Request);
            if (!user.FirstLoginTime.HasValue)
            {
                user.FirstLoginTime = DateTime.Now;
                user.FirstLoginIP = ip;
            }
            user.LastLoginTime = DateTime.Now;
            user.LastLoginIP = ip;

            Database.Save(user);
            return Result.Success("登录成功！", Utils.MapTo<UserInfo>(user));
        }
        #endregion

        #region Index
        internal void SignOut(string userName)
        {

        }

        internal List<MenuInfo> GetUserMenus(string userName, string parentId)
        {
            return Repository.GetUserMenus(Database, userName, parentId);
        }
        #endregion

        #region Welcome
        internal PagingResult<object> GetTodoLists(PagingCriteria criteria)
        {
            return new PagingResult<object>
            {
                TotalCount = 33,
                PageData = new List<object>
                {
                    new {Oid="1",Name="请假审批",Qty=1},
                    new {Oid="2",Name="费用报销",Qty=2},
                    new {Oid="3",Name="出差审批",Qty=3}
                }
            };
        }

        internal PagingResult<object> GetCompanyNews(PagingCriteria criteria)
        {
            return new PagingResult<object>
            {
                TotalCount = 55,
                PageData = new List<object>
                {
                    new {Oid="1",Title="公司新系统上线",CreateBy="管理员",CreateTime=DateTime.Now},
                    new {Oid="2",Title="关于放假通知",CreateBy="张三",CreateTime=DateTime.Now},
                    new {Oid="3",Title="关于员工福利通知",CreateBy="李四",CreateTime=DateTime.Now}
                }
            };
        }

        internal PagingResult<object> GetShortCuts()
        {
            return null;
        }
        #endregion

        #region UserInfo
        internal UserInfo GetUserInfo(string userName)
        {
            var user = Repository.GetUser(Database, userName);
            return Utils.MapTo<UserInfo>(user);
        }

        internal Result SaveUserInfo(dynamic model)
        {
            var entity = Repository.GetUser(Database, (string)model.UserName);
            if (entity == null)
                return Result.Error("当前用户不存在！");

            entity.FillModel(model);
            var vr = entity.Validate();
            if (!vr.IsValid)
                return Result.Error(vr.Message);

            Database.Save(entity);
            return Result.Success("保存成功！", entity.Id);
        }
        #endregion
    }
}
