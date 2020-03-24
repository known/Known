using System;
using System.Collections.Generic;
using System.Linq;

namespace Known.Jobs
{
    class JobService
    {
        private IJobRepository repository;

        public JobService(IJobRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public List<JobInfo> GetServerJobs(string server)
        {
            return repository.GetServerJobs(server);
        }

        public List<JobInfo> GetRestartServerJobs(string server)
        {
            var jobs = GetServerJobs(server);
            if (jobs == null || jobs.Count == 0)
                return null;

            return jobs.Where(j => j.IsRestart).ToList();
        }

        public void UpdateJob(JobInfo job)
        {
            repository.UpdateJob(job);
        }

        public void BeginJob(JobInfo job)
        {
            job.StartTime = DateTime.Now;
            job.Status = JobStatus.Running;
            job.Message = "运行中......";
            UpdateJob(job);
        }

        public void EndJob(JobInfo job, JobResult result, string logInfo)
        {
            if (result.IsSuccess)
            {
                job.SuccessCount = (job.SuccessCount ?? 0) + 1;
                AddRecord(job, RecordStatus.Success, logInfo);
            }

            job.EndTime = DateTime.Now;
            job.Status = JobStatus.Normal;
            job.Message = result.Message;
            UpdateJob(job);
        }

        public void ExceptionJob(JobInfo job, Exception ex, string logInfo)
        {
            job.FailCount = (job.FailCount ?? 0) + 1;
            AddRecord(job, RecordStatus.Failure, logInfo);

            job.EndTime = DateTime.Now;
            job.Status = JobStatus.Abnormal;
            job.Message = ex.Message;
            UpdateJob(job);
        }

        private void AddRecord(JobInfo job, RecordStatus status, string logInfo)
        {
            var record = new JobRecord
            {
                JobId = job.Id,
                Server = job.Server,
                Name = job.Name,
                ExecuteTarget = job.ExecuteTarget,
                ExecuteInterval = job.ExecuteInterval,
                Status = status,
                StartTime = job.StartTime,
                EndTime = job.EndTime,
                Message = job.Message,
                LogInfo = logInfo
            };
            repository.AddRecord(record);
        }
    }
}
