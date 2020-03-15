using System.Collections.Generic;
using System.Dynamic;

namespace Known
{
    class DynamicParameter : DynamicObject
    {
        private readonly Dictionary<string, object> datas = new Dictionary<string, object>();

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
