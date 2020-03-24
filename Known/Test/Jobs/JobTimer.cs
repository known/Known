using System.Timers;

namespace Known.Jobs
{
    public class JobTimer : Timer
    {
        public JobTimer(JobInfo job, CheckResult result)
            : base(result.TimerInterval)
        {
            Id = job.Id;
            Job = job;
            CheckResult = result;
        }

        public string Id { get; }
        public JobInfo Job { get; }
        public CheckResult CheckResult { get; }
        internal JobHelper Helper { get; set; }
    }
}
