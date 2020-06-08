using Known;
using System;
using System.Collections.Generic;

namespace KRunner
{
    internal class JobRunner
    {
        private static List<JobConfig> jobs;

        public static void Start(AppInfo info)
        {
            if (info == null)
            {
                Console.WriteLine("The appInfo is not null.");
                return;
            }

            if (info.Jobs == null || info.Jobs.Count == 0)
            {
                Console.WriteLine("No Job to run.");
                return;
            }

            jobs = info.Jobs;

            foreach (var item in jobs)
            {
                if (!item.Enabled)
                    continue;

                var type = Type.GetType(item.TypeName);
                if (type == null)
                {
                    Console.WriteLine($"The {item.TypeName} is not exists.");
                    continue;
                }

                var job = Activator.CreateInstance(type) as IJob;
                if (job == null)
                {
                    Console.WriteLine($"The {item.TypeName} is not impl the IJob.");
                    continue;
                }

                job.Config = item;
                JobHelper.StartJob(job);
            }
        }

        public static void Stop()
        {
            if (jobs == null || jobs.Count == 0)
                return;

            foreach (var item in jobs)
            {
                JobHelper.AbortJob(item.Name);
            }

            foreach (var item in jobs)
            {
                JobHelper.StopJob(item.Name);
            }
        }
    }
}
