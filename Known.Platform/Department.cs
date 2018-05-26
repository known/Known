using System.Collections.Generic;

namespace Known.Platform
{
    /// <summary>
    /// 部门。
    /// </summary>
    public class Department
    {
        /// <summary>
        /// 取得或设置ID。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 取得或设置上级公司。
        /// </summary>
        public Department Parent { get; set; }

        /// <summary>
        /// 取得或设置名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置子公司集合。
        /// </summary>
        public List<Department> Children { get; set; }
    }
}
