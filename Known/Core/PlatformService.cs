/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

namespace Known.Core
{
    public partial class PlatformService
    {
        private static AppInfo App => Config.App;
        private static AppContext Context => AppContext.Current;
        private static readonly PlatformRepository Repository = new PlatformRepository();

        private UserInfo currentUser;
        public UserInfo CurrentUser
        {
            get
            {
                if (currentUser != null)
                    return currentUser;

                return UserHelper.GetUser(out _);
            }
            internal set
            {
                currentUser = value;
            }
        }

        private Database database;
        public Database Database
        {
            get
            {
                if (database == null)
                    database = new Database();

                database.User = CurrentUser;
                return database;
            }
            internal set
            {
                database = value;
            }
        }
    }

    public partial class PlatformRepository
    {
    }
}