using Known.Data;
using Known.Extensions;
using System.Collections.Generic;
using System.Data;

namespace Known.Services
{
    public interface IUserService
    {
        List<UserInfo> GetUsers();
        UserInfo GetUser(string id);
        UserInfo GetUserByUserName(string userName);
        ValidateResult Remove(string id);
        ValidateResult Save(UserInfo user);
        ValidateResult ResetPassword(string id);
        ValidateResult ChangePassword(string id, string oldPassword, string newPassword, string newPassword1);
    }

    public class UserService : IUserService
    {
        public List<UserInfo> GetUsers()
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select * from T_Users";
            return command.ToList(r => { return GetUserInfo(r); });
        }

        public UserInfo GetUser(string id)
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select * from T_Users where Id=?Id";
            command.Parameters.Add("Id", id);
            return command.ToEntity(r => { return GetUserInfo(r); });
        }

        public UserInfo GetUserByUserName(string userName)
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select * from T_Users where UserName=?UserName";
            command.Parameters.Add("UserName", userName);
            return command.ToEntity(r => { return GetUserInfo(r); });
        }

        public ValidateResult Remove(string id)
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "delete from T_Users where Id=?Id";
            command.Parameters.Add("Id", id);
            var message = command.Execute();
            return new ValidateResult(message);
        }

        public ValidateResult Save(UserInfo user)
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select count(*) from T_Users where Id=?Id";
            command.Parameters.Add("Id", user.Id);
            if (command.ToScalar<int>() > 0)
            {
                command.Text = "update T_Users set DisplayName=?DisplayName,Email=?Email,Roles=?Roles where Id=?Id";
                command.Parameters.Add("Id", user.Id);
            }
            else
            {
                command.Text = "select count(*) from T_Users where UserName=?UserName";
                command.Parameters.Add("UserName", user.UserName);
                if (command.ToScalar<int>() > 0)
                {
                    return new ValidateResult("用户名已经存在！");
                }
                command.Text = "insert into T_Users(Id,UserName,DisplayName,Password,Email,Roles) values(?Id,?UserName,?DisplayName,?Password,?Email,?Roles)";
                command.Parameters.Add("Id", DbHelper.NewId);
                command.Parameters.Add("UserName", user.UserName);
                command.Parameters.Add("Password", "888888".ToMd5());
            }
            command.Parameters.Add("DisplayName", user.DisplayName);
            command.Parameters.Add("Email", user.Email);
            command.Parameters.Add("Roles", user.Roles);
            var message = command.Execute();
            return new ValidateResult(message);
        }

        public ValidateResult ResetPassword(string id)
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "update T_Users set Password=?Password where Id=?Id";
            command.Parameters.Add("Id", id);
            command.Parameters.Add("Password", "888888".ToMd5());
            var message = command.Execute();
            return new ValidateResult(message);
        }

        public ValidateResult ChangePassword(string id, string oldPassword, string newPassword, string newPassword1)
        {
            var user = GetUser(id);
            if (user != null && user.Password != oldPassword.ToMd5())
                return new ValidateResult("原密码不正确！");
            if (newPassword != newPassword1)
                return new ValidateResult("确认密码不一致！");

            var command = DbHelper.Default.CreateCommand();
            command.Text = "update T_Users set Password=?Password where Id=?Id";
            command.Parameters.Add("Id", id);
            command.Parameters.Add("Password", newPassword.ToMd5());
            var message = command.Execute();
            return new ValidateResult(message);
        }

        private static UserInfo GetUserInfo(DataRow row)
        {
            return new UserInfo
            {
                Id = row.Get<string>("Id"),
                UserName = row.Get<string>("UserName"),
                DisplayName = row.Get<string>("DisplayName"),
                Password = row.Get<string>("Password"),
                Email = row.Get<string>("Email"),
                Roles = row.Get<string>("Roles")
            };
        }
    }
}
