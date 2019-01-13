using System.Collections.Generic;
using System.Dynamic;
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
        public string UserName { get; set; }
        public dynamic Parameter { get; }

        public static Context Create(string userName = null)
        {
            var database = new Database();
            var logger = new FileLogger();
            return new Context(database, logger, userName);
        }
    }

    internal sealed class DynamicParameter : DynamicObject
    {
        private Dictionary<string, object> datas = new Dictionary<string, object>();

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return datas.Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            if (datas.ContainsKey(binder.Name))
            {
                result = datas[binder.Name];
            }
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            datas[binder.Name] = value;
            return true;
        }
    }
}
