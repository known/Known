namespace Known
{
    public interface IThreadJob
    {
        JobConfig Config { get; set; }
        void Run();
    }

    public class JobConfig
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public int Interval { get; set; }
        public string TypeName { get; set; }
    }
}
