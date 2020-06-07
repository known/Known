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
                Console.WriteLine("No ThreadJob to run.");
                return;
            }

            jobs = info.Jobs;

            var index = 0;
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
                    Console.WriteLine($"The {item.TypeName} is not impl the IThreadJob.");
                    continue;
                }

                Console.WriteLine($"{++index}.{item.Name} is running.");
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
                Console.WriteLine($"{item.Name} is aborting.");
                JobHelper.AbortJob(item.Name);
            }

            foreach (var item in jobs)
            {
                Console.WriteLine($"{item.Name} is stopping.");
                JobHelper.StopJob(item.Name);
            }
        }
    }
}
