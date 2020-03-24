using System.Collections.Generic;

namespace Known.Jobs
{
    public interface IJobRepository
    {
        List<JobInfo> GetServerJobs(string server);
        void UpdateJob(JobInfo job);
        void AddRecord(JobRecord record);
    }

    class JobRepository : IJobRepository
    {
        public List<JobInfo> GetServerJobs(string server)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateJob(JobInfo job)
        {
            throw new System.NotImplementedException();
        }

        public void AddRecord(JobRecord record)
        {
            throw new System.NotImplementedException();
        }
    }
}