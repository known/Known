using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Known.Core
{
    public class PlatformService
    {
        internal Result SignIn(string userName, string password)
        {
            return Result.Success("登录成功！", userName);
        }
    }
}
