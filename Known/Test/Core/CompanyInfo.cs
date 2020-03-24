using System.Collections.Generic;

namespace Known.Core
{
    /// <summary>
    /// 公司信息类。
    /// </summary>
    public class CompanyInfo
    {
        /// <summary>
        /// 取得或设置主键 Id。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 取得或设置上级公司对象。
        /// </summary>
        public CompanyInfo Parent { get; set; }

        /// <summary>
        /// 取得或设置公司名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置子公司对象列表。
        /// </summary>
        public List<CompanyInfo> Children { get; set; }
    }
}