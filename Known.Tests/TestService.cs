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

    public class ContextService : ServiceBase<IContextRepository>
    {
        public ContextService(Context context) : base(context)
        {
        }

        public string SayHello()
        {
            return Repository.SayHello();
        }

        public string SayHelloByName(string name)
        {
            return Repository.SayHello(name);
        }

        public string SayHelloByGreet(string greet, string name)
        {
            return Repository.SayHello($"{name}, {greet}");
        }
    }

    public interface IContextRepository : IRepository
    {
        string SayHello(string message = null);
    }

    class ContextRepository : TestRepository, IContextRepository
    {
        public ContextRepository() { }

        public ContextRepository(Database database) : base(database)
        {
        }

        public string SayHello(string message = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                return "Hello!";

            return $"Hello {message}!";
        }
    }
}
