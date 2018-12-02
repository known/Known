using System;
using System.Collections.Generic;
using System.IO;
using Known.Log;

namespace Known.Jobs
{
    public class JobHelper
    {
        private JobService service;

        public JobHelper(JobService service)
        {
            this.service = service;
        }

        public static void SendExceptionMail(string subject, string body)
        {
            var toMails = Setting.Instance.ExceptionMails;
            Mail.Send(toMails, subject, body);
        }

        public void StopJobs(string server)
        {
            var jobs = service.GetServerJobs(server);
            foreach (var job in jobs)
            {
                if (job.Status == JobStatus.Running)
                {
                    service.UpdateJobStatus(job, JobStatus.Normal, "强制停止运行");
                }
            }
        }

        public CheckResult CheckJob(ILogger log, JobInfo job)
        {
            var type = GetCallTargetType(log, job.ExecuteTarget);
            if (type == null)
            {
                return new CheckResult
                {
                    Pass = false,
                    ErrorMessage = "没有找到执行目标类型！"
                };
            }

            var instance = Activator.CreateInstance(type) as IJob;
            if (instance == null)
            {
                return new CheckResult
                {
                    Pass = false,
                    ErrorMessage = "执行目标实例为空，请确认Job类型是实现IJob接口。"
                };
            }

            var result = service.CheckInterval(job.ExecuteInterval);
            result.Instance = instance;
            return result;
        }

        public bool RunJob(ILogger log, JobInfo job, IJob instance, Dictionary<string, object> config)
        {
            var fileName = string.Format(@"{0}\log\Jobs\{1}\{2:yyyy}\{2:yyyyMM}\{2:yyyyMMddHHmmssfff}.log", Environment.CurrentDirectory, job.Name, DateTime.Now);
            var logger = new FileLogger(fileName);
            try
            {
                service.BeginJob(job);
                var result = instance.Execute(logger, config);
                service.EndJob(job, result, File.ReadAllText(fileName));
                return true;
            }
            catch (Exception ex)
            {
                var logContent = File.ReadAllText(fileName);
                service.ExceptionJob(job, ex, logContent);
                log.Error("执行【" + job.Name + "】作业异常。", ex);
                SendExceptionMail("服务执行异常通知", logContent);
                return false;
            }
        }

        private static Type GetCallTargetType(ILogger log, string typeName)
        {
            try
            {
                log.Info($"获取执行目标：{typeName}");
                return Type.GetType(typeName);
            }
            catch (Exception ex)
            {
                log.Error($"查找执行目标类型异常。TypeName：{typeName}", ex);
                return null;
            }
        }
    }
}
