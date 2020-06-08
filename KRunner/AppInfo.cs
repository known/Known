using Known;
using System.Collections.Generic;
using System.IO;

namespace KRunner
{
    internal class AppInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ConnName { get; set; }
        public string ApiUrl { get; set; }
        public string ProxyUrl { get; set; }
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