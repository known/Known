using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Timers;

namespace Known.Jobs
{
    public class MainService : System.ServiceProcess.ServiceBase
    {
        private static ConcurrentDictionary<string, JobTimer> timers;
        private JobHelper helper;
        private IContainer components;
        private Timer checkTimer;

        public MainService()
        {
            ServiceName = JobHelper.ServiceName;

            components = new System.ComponentModel.Container();
            timers = new ConcurrentDictionary<string, JobTimer>();
            helper = new JobHelper();
            checkTimer = new Timer(JobHelper.TimerInterval) { Enabled = true };
            checkTimer.Elapsed += CheckTimer_Elapsed;
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
            helper.StartJobs((s, j) =>
            {
                if (StartJob(s, j))
                {
                    System.Threading.Thread.Sleep(200);
                }
            });

            checkTimer.Start();
        }

        protected override void OnStop()
        {
            checkTimer.Stop();
            helper.StopJobs();
            Dispose(true);
        }

        private bool StartJob(JobService service, JobInfo job)
        {
            var result = helper.CheckJob(job);
            if (!result.IsPass)
            {
                job.Status = JobStatus.Abnormal;
                job.Message = result.ErrorMessage;
                helper.Service.UpdateJob(job);
                return false;
            }

            var timer = new JobTimer(job.Id, result.TimerInterval)
            {
                Job = job,
                CheckResult = result,
                Config = service.GetJobConfig(job.Id),
                Enabled = true
            };
            timer.Elapsed += Timer_Elapsed;
            timers[timer.Id] = timer;
            timer.Start();
            return true;
        }

        private void StopJob(JobTimer timer)
        {
            timer.Stop();
            timers.TryRemove(timer.Id, out timer);
            timer.Dispose();
        }

        private void CheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            helper.StartJobs((s, j) =>
            {
                if (!timers.ContainsKey(j.Id))
                {
                    if (StartJob(s, j))
                    {
                        System.Threading.Thread.Sleep(200);
                    }
                }
            });
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
