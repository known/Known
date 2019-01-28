using Known.Jobs;

namespace Known.Tests.Core.Jobs
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
