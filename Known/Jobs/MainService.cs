using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Timers;

namespace Known.Jobs
{
    public class MainService : System.ServiceProcess.ServiceBase
    {
        private static ConcurrentDictionary<string, JobTimer> timers;
        private IContainer components;
        private Timer checkTimer;

        public MainService()
        {
            ServiceName = JobHelper.ServiceName;

            components = new System.ComponentModel.Container();
            timers = new ConcurrentDictionary<string, JobTimer>();
            checkTimer = new Timer(JobHelper.TimerInterval) { Enabled = true };
            checkTimer.Elapsed += CheckTimer_Elapsed;
        }

        private JobHelper Helper
        {
            get { return new JobHelper(); }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
                if (timers != null)
                    timers.Clear();
                if (checkTimer != null)
                    checkTimer.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnStart(string[] args)
        {
            Helper.StartJobs((h, j) => StartJob(h, j));
            checkTimer.Start();
        }

        protected override void OnStop()
        {
            checkTimer.Stop();
            Helper.StopJobs();
            Dispose(true);
        }

        private static bool StartJob(JobHelper helper, JobInfo job)
        {
            var result = helper.CheckJob(job);
            if (!result.IsPass)
            {
                job.Status = JobStatus.Abnormal;
                job.Message = result.ErrorMessage;
                helper.UpdateJob(job);
                return false;
            }

            var timer = new JobTimer(job.Id, result.TimerInterval)
            {
                Job = job,
                CheckResult = result,
                Helper = helper,
                Enabled = true
            };
            timer.Elapsed += JobTimer_Elapsed;
            timers[timer.Id] = timer;
            timer.Start();
            return true;
        }

        private static void StopJob(JobTimer timer)
        {
            timer.Stop();
            timers.TryRemove(timer.Id, out timer);
            timer.Dispose();
        }

        private void CheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Helper.StartJobs((h, j) =>
            {
                if (!timers.ContainsKey(j.Id))
                {
                    StartJob(h, j);
                }
            });
        }

        private static void JobTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var timer = sender as JobTimer;
                var job = timer.Job;
                var result = timer.CheckResult;

                if (job.Status == JobStatus.Running)
                    return;

                if (!timer.Helper.CheckJobTime(DateTime.Now, job, result))
                    return;

                if (!timer.Helper.RunJob(job, result.Instance))
                    StopJob(timer);
            }
            catch (Exception ex)
            {
                JobHelper.SendMail("主服务执行异常", ex.ToString());
            }
        }
    }
}
