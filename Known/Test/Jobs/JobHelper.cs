using System;
using System.Collections.Generic;
using System.Linq;
using Known.Log;

namespace Known.Jobs
{
    class JobHelper
    {
        public static string ServiceName = Config.AppSetting("ServiceName");
        public static string Server = Config.AppSetting("Server");
        public static double TimerInterval = Config.AppSetting<double>("TimerInterval", 1000);

        public JobHelper()
        {
            var repository = Container.Resolve<IJobRepository>();
            Service = new JobService(repository);
        }

        private JobService Service { get; }

        public static void SendMail(string subject, string body)
        {
            var toMails = Setting.ExceptionMails;
            Mail.Send(toMails, subject, body);
        }

        public void StartJobs(Action<JobHelper, JobInfo> action)
        {
            try
            {
                var jobs = Service.GetServerJobs(Server);
                foreach (var job in jobs)
                {
                    if (job.Status == JobStatus.Normal)
                    {
                        action(this, job);
                    }
                }
            }
            catch (Exception ex)
            {
                SendMail("主服务启动异常", ex.ToString());
            }
        }

        public void RestartJobs(Action<JobHelper, JobInfo> action)
        {
            try
            {
                var jobs = Service.GetRestartServerJobs(Server);
                foreach (var job in jobs)
                {
                    job.IsRestart = false;
                    Service.UpdateJob(job);

                    if (job.Status == JobStatus.Normal)
                    {
                        action(this, job);
                    }
                }
            }
            catch (Exception ex)
            {
                SendMail("主服务启动异常", ex.ToString());
            }
        }

        public void StopJobs()
        {
            try
            {
                var jobs = Service.GetServerJobs(Server);
                foreach (var job in jobs)
                {
                    if (job.Status == JobStatus.Running)
                    {
                        job.Status = JobStatus.Normal;
                        job.Message = "强制停止运行";
                        Service.UpdateJob(job);
                    }
                }
            }
            catch (Exception ex)
            {
                SendMail("主服务停止异常", ex.ToString());
            }
        }

        public CheckResult CheckJob(JobInfo job)
        {
            var type = Type.GetType(job.ExecuteTarget);
            if (type == null)
                return FailCheckResult(job, "没有找到执行目标类型！");

            if (!(Activator.CreateInstance(type) is IJob instance))
                return FailCheckResult(job, "执行目标实例为空，请确认Job类型是实现IJob接口。");

            if (string.IsNullOrEmpty(job.ExecuteInterval))
                return FailCheckResult(job, "时间间隔不能为空！");

            var timerInterval = 1000;
            var timeFormat = string.Empty;
            var timeValues = new List<string>();

            if (job.ExecuteInterval.Contains("="))
            {
                var intervalArray = job.ExecuteInterval.Split('=');
                timeFormat = intervalArray[0];
                timeValues = intervalArray[1].Split(',').ToList();

                if (timeValues == null || timeValues.Count == 0)
                    return FailCheckResult(job, "间隔时间配置错误，没有对应的时间值。");
            }
            else
            {
                int.TryParse(job.ExecuteInterval, out timerInterval);
                if (timerInterval < 1000)
                    return FailCheckResult(job, "时间间隔不能小于1000！");
            }

            return new CheckResult
            {
                IsPass = true,
                ErrorMessage = string.Empty,
                TimerInterval = timerInterval,
                TimeFormat = timeFormat,
                TimeValues = timeValues,
                Instance = instance
            };
        }

        public bool CheckJobTime(DateTime now, JobInfo job, CheckResult result)
        {
            if (string.IsNullOrEmpty(result.TimeFormat))
                return true;

            if (result.TimeValues == null || result.TimeValues.Count == 0)
            {
                job.Status = JobStatus.Abnormal;
                job.Message = "间隔时间配置错误，没有对应的时间值。";
                Service.UpdateJob(job);
                return false;
            }

            var nowString = now.ToString(result.TimeFormat);
            return result.TimeValues.Contains(nowString);
        }

        public bool RunJob(JobInfo job, IJob instance)
        {
            var log = new ConsoleLogger();
            try
            {
                Service.BeginJob(job);
                var result = instance.Execute(log, job.Config);
                Service.EndJob(job, result, log.TraceInfo);
                return true;
            }
            catch (Exception ex)
            {
                log.Trace(ex.ToString());
                Service.ExceptionJob(job, ex, log.TraceInfo);
                SendMail("服务执行异常通知", log.TraceInfo);
                return false;
            }
        }

        private CheckResult FailCheckResult(JobInfo job, string message)
        {
            job.Status = JobStatus.Abnormal;
            job.Message = message;
            Service.UpdateJob(job);
            return new CheckResult { IsPass = false };
        }
    }
}
