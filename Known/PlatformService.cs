using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Known.Web;

namespace Known
{
    public class PlatformService
    {
        private IPlatformRepository Repository
        {
            get { return Container.Resolve<IPlatformRepository>(); }
        }

        private UserInfo CurrentUser
        {
            get { return UserHelper.GetUser(out _); }
        }

        private Database database;
        private Database Database
        {
            get
            {
                if (database == null)
                    database = new Database();
                database.User = CurrentUser;
                return database;
            }
        }

        public Result SignIn(string userName, string password)
        {
            var entity = Repository.GetUser(Database, userName);
            if (entity == null)
                return Result.Error("用户名不存在！");

            if (entity.Enabled == 0)
                return Result.Error("用户已禁用！");

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

            var user = GetUserInfo(entity);
            user.Token = Utils.GetGuid();
            UserHelper.SetUser(user);
            Repository.UpdateUser(Database, entity);
            return Result.Success("登录成功！", user);
        }

        public void SignOut(string userName)
        {
        }

        public UserInfo GetUserInfo(string userName)
        {
            var user = UserHelper.GetUser(out _);
            if (user == null && !string.IsNullOrWhiteSpace(userName))
            {
                var entity = Repository.GetUser(Database, userName);
                user = GetUserInfo(entity);
                UserHelper.SetUser(user);
            }
            return user;
        }

        public List<MenuInfo> GetUserMenus(string userName)
        {
            if (userName == "System")
            {
                var menus = Repository.GetMenus(Database);
                var devMenus = Cache.Get<List<MenuInfo>>("DevMenu");
                if (devMenus != null && devMenus.Count > 0)
                    menus.InsertRange(0, devMenus);
                return menus;
            }

            return Repository.GetUserMenus(Database, userName);
        }

        public Dictionary<string, List<CodeInfo>> GetCodes(string compNo)
        {
            var codes = Cache.GetCodes();
            var data = Repository.GetCodes(Database, compNo);
            if (data != null && data.Count > 0)
                codes.AddRange(data);

            var dics = new Dictionary<string, List<CodeInfo>>();
            var cates = codes.Select(c => c.Category).Distinct();
            foreach (var item in cates)
            {
                var lists = codes.Where(c => c.Category == item).ToList();
                dics.Add(item, lists);
            }

            return dics;
        }

        public Result SaveUserInfo(UserInfo model)
        {
            var entity = Repository.GetUserById(Database, (string)model.Id);
            if (entity == null)
                return Result.Error("当前用户不存在！");

            entity.Name = model.Name;
            entity.EnglishName = model.EnglishName;
            entity.Gender = model.Gender;
            entity.Phone = model.Phone;
            entity.Mobile = model.Mobile;
            entity.Email = model.Email;
            entity.Note = model.Note;
            Repository.UpdateUser(Database, entity);
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

            var entity = Repository.GetUserById(Database, user.Id);
            if (entity == null)
                return Result.Error("当前用户不存在！");

            var pwd = Utils.ToMd5(oldPassword);
            if (entity.Password != pwd)
                return Result.Error("当前密码不正确！");

            entity.Password = Utils.ToMd5(password);
            Repository.UpdatePassword(Database, entity.Id, entity.Password);
            return Result.Success("修改成功！", entity.Id);
        }

        private UserInfo GetUserInfo(UserInfo user)
        {
            user.CompName = Config.CompName;
            user.OrgName = Repository.GetOrgName(Database, Config.CompNo, user.OrgNo);
            user.IsOrgGroup = string.IsNullOrWhiteSpace(user.OrgName);
            return user;
        }

        public void AddLog(Database db, string type, string target, string content)
        {
            var user = CurrentUser;
            Repository.AddLog(db, user, type, target, content);
        }
    }

    public interface IPlatformRepository
    {
        UserInfo GetUser(Database db, string userName);
        UserInfo GetUserById(Database db, string id);
        List<MenuInfo> GetMenus(Database db);
        List<MenuInfo> GetUserMenus(Database db, string userName);
        List<CodeInfo> GetCodes(Database db, string compNo);
        string GetOrgName(Database db, string compNo, string orgNo);
        void UpdateUser(Database db, UserInfo user);
        void UpdatePassword(Database db, string id, string password);
        void AddLog(Database db, UserInfo user, string type, string target, string content);
    }

    public class MenuInfo
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public string Target { get; set; }
        public bool Checked { get; set; }
        public int Order { get; set; }
    }

    public class CodeInfo
    {
        public string Category { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    class PlatformRepository : IPlatformRepository
    {
        public UserInfo GetUser(Database db, string userName)
        {
            var sql = "select * from SysUser where UserName=@userName";
            return db.QuerySingle<UserInfo>(sql, new { userName });
        }

        public UserInfo GetUserById(Database db, string id)
        {
            var sql = "select * from SysUser where Id=@id";
            return db.QuerySingle<UserInfo>(sql, new { id });
        }

        public List<MenuInfo> GetMenus(Database db)
        {
            var sql = "select * from SysModule where Enabled=1 order by Sort";
            return db.QueryList<MenuInfo>(sql);
        }

        public List<MenuInfo> GetUserMenus(Database db, string userName)
        {
            var sql = @"
select * from SysModule 
where Enabled=1 and Id in (
  select a.ModuleId from SysRoleModule a,SysRole b,SysUserRole c,SysUser d
  where a.RoleId=b.Id and b.Id=c.RoleId and c.UserId=d.Id and d.UserName=@userName
  union 
  select a.ModuleId from SysUserModule a,SysUser b 
  where a.UserId=b.Id and b.UserName=@userName
)
order by Sort";
            return db.QueryList<MenuInfo>(sql, new { userName });
        }

        public List<CodeInfo> GetCodes(Database db, string compNo)
        {
            var sql = "select * from SysDictionary where Enabled=1 and CompNo=@compNo order by Category,Sort";
            return db.QueryList<CodeInfo>(sql, new { compNo });
        }

        public string GetOrgName(Database db, string compNo, string orgNo)
        {
            var sql = "select Name from SysOrganization where CompNo=@compNo and Code=@orgNo";
            return db.Scalar<string>(sql, new { compNo, orgNo });
        }

        public void UpdateUser(Database db, UserInfo user)
        {
            var sql = @"
update SysUser 
set FirstLoginTime=@FirstLoginTime,FirstLoginIP=@FirstLoginIP
   ,LastLoginTime=@LastLoginTime,LastLoginIP=@LastLoginIP
   ,Name=@Name,EnglishName=@EnglishName,Gender=@Gender,Phone=@Phone
   ,Mobile=@Mobile,Email=@Email,Note=@Note 
where Id=@Id";
            db.Execute(sql, user);
        }

        public void UpdatePassword(Database db, string id, string password)
        {
            var sql = "update SysUser set Password=@password where Id=@Id";
            db.Execute(sql, new { password, id });
        }

        public void AddLog(Database db, UserInfo user, string type, string target, string content)
        {
            var sql = @"
insert into SysLog(Id,CreateBy,CreateTime,Version,CompNo,Type,Target,Content) 
values(@Id,@CreateBy,@CreateTime,1,@CompNo,@type,@target,@content)";
            db.Execute(sql, new
            {
                Id = Utils.GetGuid(),
                CreateBy = user.UserName,
                CreateTime = DateTime.Now,
                user.CompNo,
                type,
                target,
                content
            });
        }
    }
}
