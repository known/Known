using System.Collections.Generic;
using System.Web.Http;

namespace Known.WebApi
{
    public class ValuesController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new string[] { "111", "222" };
        }
    }
}
