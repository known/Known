using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Known.Runner
{
    internal class AppInfo
    {
        public string Name { get; set; }
        public List<JobConfig> Jobs { get; set; }

        internal static AppInfo Load(FileInfo file)
        {
            if (file == null || !file.Exists)
                return null;

            using (var sr = file.OpenText())
            {
                var json = sr.ReadToEnd();
                return Utils.FromJson<AppInfo>(json);
            }
        }
    }
}