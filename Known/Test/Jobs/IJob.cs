using System.Collections.Generic;
using Known.Log;

namespace Known.Jobs
{
    public interface IJob
    {
        JobResult Execute(ILogger log, Dictionary<string, object> config);
    }
}
