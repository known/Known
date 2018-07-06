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
            Database = database;
            if (Database != null)
            {
                Database.UserName = userName;
            }

            Logger = logger;
            UserName = userName;
        }

        public Database Database { get; }
        public ILogger Logger { get; }
        public string UserName { get; }
        public dynamic Param { get; set; }
    }
}
