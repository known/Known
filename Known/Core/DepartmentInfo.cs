using System.Collections.Generic;

namespace Known.Core
{
    /// <summary>
    /// 部门信息类。
    /// </summary>
    public class DepartmentInfo
    {
        /// <summary>
        /// 取得或设置主键 Id。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 取得或设置上级部门对象。
        /// </summary>
        public DepartmentInfo Parent { get; set; }

        /// <summary>
        /// 取得或设置部门名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置部门管理者对象。
        /// </summary>
        public UserInfo Manager { get; set; }

        /// <summary>
        /// 取得或设置子部门对象列表。
        /// </summary>
        public List<DepartmentInfo> Children { get; set; }
    }
}