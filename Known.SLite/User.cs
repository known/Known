using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Known.SLite
{
    public enum UserType
    {
        Administrator = 0,
        CommonUser = 1
    }

    public class User : Entity<User>
    {
        public User()
        {
            UserType = UserType.CommonUser;
            Enabled = true;
            DisplayOrder = 1000;
        }

        public UserType UserType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int DisplayOrder { get; set; }
        public bool Enabled { get; set; }

        public override List<string> Validate()
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(UserName))
                list.Add("用户名不能为空！");
            if (!string.IsNullOrEmpty(Email) && !Email.IsEmail())
                list.Add("邮箱格式不正确！");
            return list;
        }
    }
}
