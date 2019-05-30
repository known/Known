using Known.Jobs;

namespace Known.Tests.Jobs
{
    public class MainServiceTest
    {
        public static void Run()
        {
            Container.Register<IJobRepository, JobRepository>();
            //MainService.Run();
        }
    }
}
