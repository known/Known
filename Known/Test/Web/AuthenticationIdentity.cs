using System.Security.Principal;

namespace Known.Web
{
    /// <summary>
    /// 用户身份认证标识类。
    /// </summary>
    public class AuthenticationIdentity : GenericIdentity
    {
        /// <summary>
        /// 初始化一个用户身份认证标识类的实例。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public AuthenticationIdentity(string name, string type)
            : base(name, type)
        {
        }

        /// <summary>
        /// 取得用户身份认证的密码。
        /// </summary>
        public string Password { get; internal set; }

        /// <summary>
        /// 取得用户身份认证的票据。
        /// </summary>
        public string Token { get; internal set; }
    }
}
