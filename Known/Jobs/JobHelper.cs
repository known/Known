using System;
using System.Collections.Generic;
using System.IO;
using Known.Log;

namespace Known.Jobs
{
    class JobHelper
    {
        public static string ServiceName = Config.AppSetting("ServiceName");
        public static string Server = Config.AppSetting("Server");
        public static double TimerInterval = Config.AppSetting<double>("TimerInterval");

        public JobHelper()
        {
            Service = new JobService();
        }

        public JobService Service { get; }

        public static void SendMail(string subject, string body)
        {
            var toMails = Setting.Instance.ExceptionMails;
            Mail.Send(toMails, subject, body);
        }

        public void StartJobs(Action<JobService, JobInfo> action)
        {
            try
            {
                var jobs = Service.GetServerJobs(Server);
                foreach (var job in jobs)
                {
                    if (job.Status == JobStatus.Normal)
                    {
                        action(Service, job);
                    }
                }
            }
            catch (Exception ex)
            {
                SendMail("主服务启动异常", ex.ToString());
            }
        }

        public void RestartJobs(Action<JobService, JobInfo> action)
        {
            try
            {
                var jobs = Service.GetRestartServerJobs(Server);
                foreach (var job in jobs)
                {
                    //Service.UpdateStarted(job);

                    if (job.Status == JobStatus.Normal)
                    {
                        action(Service, job);
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
            var type = GetCallTargetType(job.ExecuteTarget);
            if (type == null)
                return CheckResult.Fail("没有找到执行目标类型！");

            if (!(Activator.CreateInstance(type) is IJob instance))
                return CheckResult.Fail("执行目标实例为空，请确认Job类型是实现IJob接口。");

            var result = Service.CheckInterval(job.ExecuteInterval);
            result.Instance = instance;
            return result;
        }

        public bool RunJob(ILogger log, JobInfo job, IJob instance, Dictionary<string, object> config)
        {
            var fileName = string.Format(@"{0}\log\Jobs\{1}\{2:yyyy}\{2:yyyyMM}\{2:yyyyMMddHHmmssfff}.log", Environment.CurrentDirectory, job.Name, DateTime.Now);
            var logger = new FileLogger(fileName);
            try
            {
                Service.BeginJob(job);
                var result = instance.Execute(logger, config);
                Service.EndJob(job, result, File.ReadAllText(fileName));
                return true;
            }
            catch (Exception ex)
            {
                var logContent = File.ReadAllText(fileName);
                Service.ExceptionJob(job, ex, logContent);
                log.Error("执行【" + job.Name + "】作业异常。", ex);
                SendMail("服务执行异常通知", logContent);
                return false;
            }
        }

        private static Type GetCallTargetType(string typeName)
        {
            try
            {
                //log.Info($"获取执行目标：{typeName}");
                return Type.GetType(typeName);
            }
            catch (Exception ex)
            {
                //log.Error($"查找执行目标类型异常。TypeName：{typeName}", ex);
                return null;
            }
        }
    }
}
