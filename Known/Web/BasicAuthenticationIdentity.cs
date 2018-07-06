using System.Security.Principal;

namespace Known.Web
{
    public class BasicAuthenticationIdentity : GenericIdentity
    {
        public BasicAuthenticationIdentity(string name, string password)
            : base(name, "Basic")
        {
            Password = password;
        }

        public string Password { get; }
    }
}
