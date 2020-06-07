using System.Collections.Generic;

namespace Known
{
    public interface IJob
    {
        Database Database { get; set; }
        JobConfig Config { get; set; }
        void Run();
    }

    public abstract class JobBase : IJob
    {
        public Database Database { get; set; }
        public JobConfig Config { get; set; }

        protected abstract void Runing();

        public void Run()
        {
            Runing();
        }
    }

    public class JobConfig
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public int Interval { get; set; }
        public string RunTime { get; set; }
        public string TypeName { get; set; }
        public Dictionary<string, object> Params { get; set; }
    }
}
