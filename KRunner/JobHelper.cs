using System.Collections.Generic;
using Known;

namespace KRunner
{
    internal class JobHelper
    {
        private static readonly Dictionary<string, IJober> jobs = new Dictionary<string, IJober>();

        public static void StartJob(IJob job)
        {
            if (job == null || job.Config == null || string.IsNullOrWhiteSpace(job.Config.Name))
                return;

            if (!jobs.ContainsKey(job.Config.Name))
            {
                job.Load();

                if (string.IsNullOrWhiteSpace(job.Config.RunTime))
                    jobs.Add(job.Config.Name, new ThreadJober(job));
                else
                    jobs.Add(job.Config.Name, new TimerJober(job));
            }
        }

        public static void AbortJob(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;

            if (!jobs.ContainsKey(name))
                return;

            jobs[name].IsAbort = true;
        }

        public static void StopJob(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;

            if (!jobs.ContainsKey(name))
                return;

            var job = jobs[name];
            if (job.IsRunOver)
                job.Abort();
            else
                job.Join();

            jobs.Remove(name);
        }
    }
}
