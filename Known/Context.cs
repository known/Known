using Known.Data;
using Known.Log;

namespace Known
{
    public class Context
    {
        public Context(string userName)
        {
            Database = new Database { UserName = userName };
            Logger = new FileLogger();
            UserName = userName;
            Param = new DynamicParam();
        }

        public Context(ILogger logger) : this(null, logger) { }

        public Context(Database database, ILogger logger) : this(database, logger, null) { }

        public Context(Database database, ILogger logger, string userName)
        {
            Database = database;
            if (Database != null)
            {
                Database.UserName = userName;
            }

            Logger = logger;
            UserName = userName;
            Param = new DynamicParam();
        }

        public Database Database { get; }
        public ILogger Logger { get; }
        public string UserName { get; }
        public dynamic Param { get; }
    }
}
