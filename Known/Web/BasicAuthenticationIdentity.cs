using System.Security.Principal;

namespace Known.Web
{
    /// <summary>
    /// 基本用户身份认证。
    /// </summary>
    public class BasicAuthenticationIdentity : GenericIdentity
    {
        /// <summary>
        /// 构造函数，创建一个基本用户身份认证实例。
        /// </summary>
        /// <param name="name">用户名。</param>
        /// <param name="password">密码。</param>
        public BasicAuthenticationIdentity(string name, string password)
            : base(name, "Basic")
        {
            Password = password;
        }

        /// <summary>
        /// 取得密码。
        /// </summary>
        public string Password { get; }
    }
}
