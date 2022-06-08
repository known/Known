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
    public partial class SystemService : ServiceBase, IService
    {
        private static ISystemRepository Repository => Container.Resolve<ISystemRepository>(new SystemRepository());
        internal const string DevId = "DEV";
    }

    public partial interface ISystemRepository
    {
    }

    partial class SystemRepository : ISystemRepository
    {
    }
}