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
            Database = database;
            if (Database != null)
            {
                Database.UserName = userName;
            }

            Logger = logger;
            UserName = userName;
            Param = new DynamicDictionary();
        }

        public Database Database { get; }
        public ILogger Logger { get; }
        public string UserName { get; }
        public dynamic Param { get; }
    }

    internal sealed class DynamicDictionary : DynamicObject
    {
        private Dictionary<string, object> datas = new Dictionary<string, object>();

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return datas.Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = datas[binder.Name];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            datas[binder.Name] = value;
            return true;
        }
    }
}
