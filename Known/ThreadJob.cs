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
        public string TypeName { get; set; }
    }
}
