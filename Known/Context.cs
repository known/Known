using Known.Data;
using Known.Log;

namespace Known
{
    public class Context
    {
        public Context(ILogger logger) : this(null, logger) { }

        public Context(Database database, ILogger logger) : this(database, logger, null) { }

        public Context(Database database, ILogger logger, string userName)
        {
            if (database != null)
            {
                database.UserName = userName;
            }

            Database = database;
            Logger = logger;
            UserName = userName;
            Parameter = new DynamicParameter();
        }

        public Database Database { get; }
        public ILogger Logger { get; }
        public string UserName { get; }
        public dynamic Parameter { get; }

        public static Context Create(string userName)
        {
            var database = new Database();
            var logger = new FileLogger();
            return new Context(database, logger, userName);
        }
    }
}
