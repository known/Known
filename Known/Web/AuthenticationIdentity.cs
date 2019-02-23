using System.Security.Principal;

namespace Known.Web
{
    public class AuthenticationIdentity : GenericIdentity
    {
        public AuthenticationIdentity(string name, string type)
            : base(name, type)
        {
        }

        public string Password { get; internal set; }
        public string Token { get; internal set; }
    }
}
