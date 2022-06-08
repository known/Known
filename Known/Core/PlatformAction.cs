using System;
using System.Collections.Generic;
using Known.Entities;

namespace Known.Core
{
    public sealed class PlatformAction
    {
        private PlatformAction() { }

        #region Flow
        private static readonly Dictionary<string, Action<Database, FlowActionInfo>> flowActions = new Dictionary<string, Action<Database, FlowActionInfo>>();
        public static void RegisterFlow(string flowCode, Action<Database, FlowActionInfo> action)
        {
            flowActions[flowCode] = action;
        }

        internal static void SetBizFlowStatus(Database db, SysFlow info, string note)
        {
            if (!flowActions.ContainsKey(info.FlowCode))
                return;

            flowActions[info.FlowCode]?.Invoke(db, new FlowActionInfo
            {
                BizId = info.BizId,
                BizStatus = info.BizStatus,
                Note = note
            });
        }
        #endregion

        #region Dictionary
        private static readonly Dictionary<string, Func<Database, string, List<CodeInfo>>> dictionaryActions = new Dictionary<string, Func<Database, string, List<CodeInfo>>>();
        public static void RegisterDictionary(string appId, Func<Database, string, List<CodeInfo>> action)
        {
            dictionaryActions[appId] = action;
        }

        public static List<CodeInfo> GetDictionaries(Database db, string compNo, string appId)
        {
            if (!dictionaryActions.ContainsKey(appId))
                return null;

            return dictionaryActions[appId]?.Invoke(db, compNo);
        }
        #endregion

        #region Organization
        private static readonly Dictionary<string, Action<Database, OrganizationInfo>> organizationActions = new Dictionary<string, Action<Database, OrganizationInfo>>();
        public static void RegisterOrganization(string appId, Action<Database, OrganizationInfo> action)
        {
            organizationActions[appId] = action;
        }

        internal static void SetBizOrganization(Database db, UserInfo user, SysOrganization entity)
        {
            if (!organizationActions.ContainsKey(user.AppId))
                return;

            organizationActions[user.AppId]?.Invoke(db, new OrganizationInfo
            {
                CompNo = user.CompNo,
                CompName = user.CompName,
                Code = entity.Code,
                Name = entity.Name,
                Note = entity.Note
            });
        }
        #endregion

        #region User
        private static readonly Dictionary<string, Action<Database, UserInfo>> userActions = new Dictionary<string, Action<Database, UserInfo>>();
        public static void RegisterUser(string appId, Action<Database, UserInfo> action)
        {
            userActions[appId] = action;
        }

        internal static void SetBizUser(Database db, SysUser entity)
        {
            if (!userActions.ContainsKey(entity.AppId))
                return;

            var info = Utils.MapTo<UserInfo>(entity);
            userActions[entity.AppId]?.Invoke(db, info);
        }
        #endregion
    }
}