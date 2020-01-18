using System.Collections.Generic;

namespace Known.Core
{
    /// <summary>
    /// 应用程序信息类。
    /// </summary>
    public class AppInfo
    {
        /// <summary>
        /// 创建一个应用程序信息类实例。
        /// </summary>
        public AppInfo()
        {
            Modules = new List<ModuleInfo>();
            Pages = new List<ModuleInfo>();
        }

        /// <summary>
        /// 取得或设置主键 Id。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 取得或设置应用程序编码。
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 取得或设置应用程序名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置应用程序版本。
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 取得或设置应用程序描述。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 取得应用程序模块信息列表。
        /// </summary>
        public List<ModuleInfo> Modules { get; }

        /// <summary>
        /// 取得应用程序页面信息列表。
        /// </summary>
        public List<ModuleInfo> Pages { get; }

        /// <summary>
        /// 添加模块信息。
        /// </summary>
        /// <param name="info">模块信息。</param>
        public void AddModule(ModuleInfo info)
        {
            info.AppId = Id;
            info.ParentId = Id;
            Modules.Add(info);
        }
    }
}