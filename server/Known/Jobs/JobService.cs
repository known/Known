using System;
using System.Collections.Generic;
using System.Linq;

namespace Known.Jobs
{
    public class JobService
    {
        private IJobRepository repository;

        public JobService(IJobRepository repository)
        {
            this.repository = repository;
        }

        public CheckResult CheckInterval(string interval)
        {
            if (string.IsNullOrEmpty(interval))
                return new CheckResult { IsPass = false, ErrorMessage = "时间间隔不能为空！" };

            var timerInterval = 1000;
            var timeFormat = string.Empty;
            var timeValues = new List<string>();

            if (interval.Contains("="))
            {
                var intervalArray = interval.Split('=');
                timeFormat = intervalArray[0];
                timeValues = intervalArray[1].Split(',').ToList();

                if (timeValues == null || timeValues.Count == 0)
                    return new CheckResult { IsPass = false, ErrorMessage = "间隔时间配置错误，没有对应的时间值。" };
            }
            else
            {
                int.TryParse(interval, out timerInterval);
                if (timerInterval < 1000)
                    return new CheckResult { IsPass = false, ErrorMessage = "时间间隔不能小于1000！" };
            }

            return new CheckResult
            {
                IsPass = true,
                ErrorMessage = string.Empty,
                TimerInterval = timerInterval,
                TimeFormat = timeFormat,
                TimeValues = timeValues
            };
        }

        public bool CheckJobTime(DateTime now, JobInfo job, CheckResult result)
        {
            if (string.IsNullOrEmpty(result.TimeFormat))
                return true;

            if (result.TimeValues == null || result.TimeValues.Count == 0)
            {
                UpdateJobStatus(job, JobStatus.Abnormal, "间隔时间配置错误，没有对应的时间值。");
                return false;
            }

            var nowString = now.ToString(result.TimeFormat);
            return result.TimeValues.Contains(nowString);
        }

        public Dictionary<string, object> GetJobConfig(string id)
        {
            return repository.GetJobConfig(id);
        }

        public List<JobInfo> GetServerJobs(string server)
        {
            return repository.GetServerJobs(server);
        }

        public void BeginJob(JobInfo job)
        {
            job.StartTime = DateTime.Now;
            job.Status = JobStatus.Running;
            job.Message = "运行中......";
            repository.UpdateJob(job);
        }

        public void EndJob(JobInfo job, ExecuteResult result, string logInfo)
        {
            if (result.IsSuccess)
            {
                job.SuccessCount = (job.SuccessCount ?? 0) + 1;
                AddRecord(job, RecordStatus.Success, logInfo);
            }

            job.EndTime = DateTime.Now;
            job.Status = JobStatus.Normal;
            job.Message = result.Message;
            repository.UpdateJob(job);
        }

        public void ExceptionJob(JobInfo job, Exception ex, string logInfo)
        {
            job.FailCount = (job.FailCount ?? 0) + 1;
            AddRecord(job, RecordStatus.Failure, logInfo);

            job.EndTime = DateTime.Now;
            job.Status = JobStatus.Abnormal;
            job.Message = ex.Message;
            repository.UpdateJob(job);
        }

        public void UpdateJobStatus(JobInfo job, JobStatus status, string message)
        {
            job.Status = status;
            job.Message = message;
            repository.UpdateJob(job);
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
