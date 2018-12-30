using System.Collections.Generic;

namespace Known.Jobs
{
    public interface IJobRepository
    {
        List<JobInfo> GetServerJobs(string server);
        Dictionary<string, object> GetJobConfig(string id);
        void UpdateJob(JobInfo job);
        void AddRecord(JobRecord record);
    }
}