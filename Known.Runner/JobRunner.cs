using System;
using System.Collections.Generic;
using System.IO;

namespace Known.Runner
{
    internal class JobRunner
    {
        private static List<JobConfig> jobs;

        public static void Start()
        {
            Console.WriteLine("Start loading config...");
            var file = new FileInfo("app.json");
            var info = AppInfo.Load(file);
            if (info == null)
            {
                Console.WriteLine("The app.json is not exists.");
                return;
            }

            if (info.Jobs == null || info.Jobs.Count == 0)
            {
                Console.WriteLine("No ThreadJob to run.");
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

                var job = Activator.CreateInstance(type) as IThreadJob;
                if (job == null)
                {
                    Console.WriteLine($"The {item.TypeName} is not impl the IThreadJob.");
                    continue;
                }

                Console.WriteLine($"{item.Name} is running.");
                job.Config = item;
                ThreadJobHelper.StartJob(job);
            }
        }

        public static void Stop()
        {
            if (jobs == null || jobs.Count == 0)
                return;

            foreach (var item in jobs)
            {
                Console.WriteLine($"{item.Name} is aborting.");
                ThreadJobHelper.AbortJob(item.Name);
            }

            foreach (var item in jobs)
            {
                Console.WriteLine($"{item.Name} is stopping.");
                ThreadJobHelper.StopJob(item.Name);
            }
        }
    }
}
