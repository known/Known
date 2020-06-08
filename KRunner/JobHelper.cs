using System;
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
                try
                {
                    job.Load();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}: {1}", job.Config.Name, ex);
                    return;
                }

                Console.WriteLine("{0} is started.", job.Config.Name);
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

            Console.WriteLine("{0} is aborted.", name);
            jobs[name].IsAbort = true;
        }

        public static void StopJob(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;

            if (!jobs.ContainsKey(name))
                return;

            Console.WriteLine("{0} is stoped.", name);
            var job = jobs[name];
            if (job.IsRunOver)
                job.Abort();
            else
                job.Join();

            jobs.Remove(name);
        }
    }
}
