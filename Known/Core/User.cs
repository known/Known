using System.Collections.Generic;

namespace Known.Core
{
    /// <summary>
    /// 用户信息。
    /// </summary>
    public class User
    {
        /// <summary>
        /// 取得或设置登录名。
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 取得或设置姓名。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置电子邮箱。
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 取得或设置手机电话。
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 取得或设置座机电话。
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 取得或设置身份认证Token。
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 取得或设置所属公司。
        /// </summary>
        public Company Company { get; set; }

        /// <summary>
        /// 取得或设置所属部门。
        /// </summary>
        public Department Department { get; set; }

        /// <summary>
        /// 取得或设置直属上级。
        /// </summary>
        public User Manager { get; set; }

        /// <summary>
        /// 取得或设置代理人集合。
        /// </summary>
        public List<User> Proxys { get; set; }

        /// <summary>
        /// 取得或设置所拥有权限的应用程序集合。
        /// </summary>
        public List<Application> Applications { get; set; }
    }
}
