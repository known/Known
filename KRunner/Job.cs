using Known;
using System;
using System.Threading;
using System.Timers;

namespace KRunner
{
    interface IJober
    {
        string Name { get; }
        bool IsRunOver { get; }
        bool IsAbort { get; set; }
        void Abort();
        void Join();
    }

    internal class ThreadJober : IJober
    {
        private readonly Thread thread;

        public ThreadJober(IJob job)
        {
            Job = job;
            Name = job.Config.Name;
            Interval = job.Config.Interval;
            thread = new Thread(Run) { Name = Name, IsBackground = true };
            thread.Start();
        }

        public IJob Job { get; }
        public string Name { get; }
        public int Interval { get; }
        public bool IsRunOver { get; private set; } = true;
        public bool IsAbort { get; set; } = false;

        public void Abort()
        {
            thread.Abort();
        }

        public void Join()
        {
            thread.Join();
        }

        private void Run()
        {
            try
            {
                while (!IsAbort)
                {
                    IsRunOver = false;
                    Job.Run();
                    IsRunOver = true;
                    Thread.Sleep(Interval);
                }
            }
            catch (ThreadAbortException)
            {
                Logger.Info($"The thread {Name} is abort by manual.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }

    internal class TimerJober : IJober
    {
        private readonly System.Timers.Timer timer;

        public TimerJober(IJob job)
        {
            Job = job;
            Name = job.Config.Name;
            RunTime = job.Config.RunTime;
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
            timer.Start();
        }

        public IJob Job { get; }
        public string Name { get; }
        public string RunTime { get; }
        public bool IsRunOver { get; private set; } = true;
        public bool IsAbort { get; set; } = false;

        public void Abort()
        {
            timer.Enabled = false;
            timer.Stop();
        }

        public void Join()
        {
            timer.Enabled = false;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsAbort)
                return;

            try
            {
                var now = DateTime.Now;
                if (IsRunOver && CheckRunTime(RunTime, now))
                {
                    IsRunOver = false;
                    Job.Run();
                    IsRunOver = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private static bool CheckRunTime(string runTime, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(runTime))
            {
                Console.WriteLine($"{runTime} is invalid.");
                return false;
            }

            var times = runTime.Split('=');
            if (times.Length != 2)
            {
                Console.WriteLine($"{runTime} is invalid.");
                return false;
            }

            return now.ToString(times[0]) == times[1];
        }
    }
}
