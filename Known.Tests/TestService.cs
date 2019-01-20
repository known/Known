using Known.Data;

namespace Known.Tests
{
    public abstract class BaseService
    {
    }

    public interface ITestService
    {
        string Hello();
    }

    public class TestService : BaseService, ITestService
    {
        private readonly string name;

        public TestService() { }

        public TestService(string name)
        {
            this.name = name;
        }

        public string Hello()
        {
            return "Hello!";
        }
    }

    public interface INameTestService
    {
        string Hello();
    }

    public class NameTestService : BaseService, INameTestService
    {
        private readonly string name;

        public NameTestService() { }

        public NameTestService(string name)
        {
            this.name = name;
        }

        public string Hello()
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Hello!";

            return $"Hello {name}!";
        }
    }

    public class ContextService : ServiceBase
    {
        public ContextService(Context context) : base(context)
        {
        }
    }

    public interface IContextRepository : IRepository
    {
    }

    class TestContextRepository : TestRepository, IContextRepository
    {
    }
}
