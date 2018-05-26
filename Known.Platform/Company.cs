using System.Collections.Generic;

namespace Known.Platform
{
    /// <summary>
    /// 公司。
    /// </summary>
    public class Company
    {
        /// <summary>
        /// 取得或设置ID。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 取得或设置上级公司。
        /// </summary>
        public Company Parent { get; set; }

        /// <summary>
        /// 取得或设置名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置子公司集合。
        /// </summary>
        public List<Company> Children { get; set; }
    }
}
