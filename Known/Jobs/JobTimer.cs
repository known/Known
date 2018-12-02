using System.Collections.Generic;
using System.Timers;

namespace Known.Jobs
{
    public class JobTimer : Timer
    {
        public JobTimer(string id)
        {
            Id = id;
        }

        public JobTimer(string id, double interval) : base(interval)
        {
            Id = id;
        }

        public string Id { get; }
        public JobInfo Job { get; set; }
        public CheckResult CheckResult { get; set; }
        public Dictionary<string, object> Config { get; set; }
    }
}
