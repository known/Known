using System;
using System.Collections.Generic;
using System.Linq;
using Known.Log;

namespace Known.Jobs
{
    public class JobHelper
    {
        public static string ServiceName = Config.AppSetting("ServiceName");
        public static string Server = Config.AppSetting("Server");
        public static double TimerInterval = Config.AppSetting<double>("TimerInterval");

        public JobHelper()
        {
            Service = new JobService();
        }

        private JobService Service { get; }

        public static void SendMail(string subject, string body)
        {
            var toMails = Setting.Instance.ExceptionMails;
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
                    //Service.UpdateStarted(job);

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
            var type = GetCallTargetType(job.ExecuteTarget);
            if (type == null)
                return CheckResult.Fail("没有找到执行目标类型！");

            if (!(Activator.CreateInstance(type) is IJob instance))
                return CheckResult.Fail("执行目标实例为空，请确认Job类型是实现IJob接口。");

            var result = CheckInterval(job.ExecuteInterval);
            result.Instance = instance;
            return result;
        }

        public bool CheckJobTime(DateTime now, JobInfo job, CheckResult result)
        {
            if (string.IsNullOrEmpty(result.TimeFormat))
                return true;

            if (result.TimeValues == null || result.TimeValues.Count == 0)
            {
                job.Status = JobStatus.Abnormal;
                job.Message = "间隔时间配置错误，没有对应的时间值。";
                UpdateJob(job);
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

        public void UpdateJob(JobInfo job)
        {
            Service.UpdateJob(job);
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

        private CheckResult CheckInterval(string interval)
        {
            if (string.IsNullOrEmpty(interval))
                return CheckResult.Fail("时间间隔不能为空！");

            var timerInterval = 1000;
            var timeFormat = string.Empty;
            var timeValues = new List<string>();

            if (interval.Contains("="))
            {
                var intervalArray = interval.Split('=');
                timeFormat = intervalArray[0];
                timeValues = intervalArray[1].Split(',').ToList();

                if (timeValues == null || timeValues.Count == 0)
                    return CheckResult.Fail("间隔时间配置错误，没有对应的时间值。");
            }
            else
            {
                int.TryParse(interval, out timerInterval);
                if (timerInterval < 1000)
                    return CheckResult.Fail("时间间隔不能小于1000！");
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
    }
}
