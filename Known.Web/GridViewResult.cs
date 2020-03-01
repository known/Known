using System.Reflection;
using Known.Web.Mvc;

namespace Known.Web
{
    /// <summary>
    /// 网格视图结果类。
    /// </summary>
    public class GridViewResult : ViewResult
    {
        /// <summary>
        /// 初始化一个网格视图结果类的实例。
        /// </summary>
        /// <param name="context">控制器上下文对象。</param>
        /// <param name="isPartial">是否是部分视图。</param>
        public GridViewResult(ControllerContext context, bool isPartial = false) : base(context, isPartial)
        {
        }

        /// <summary>
        /// 获取View页面内容。
        /// </summary>
        /// <param name="assembly">程序集。</param>
        /// <returns>View页面内容。</returns>
        protected override string GetContent(Assembly assembly)
        {
            return GetResource(assembly, "Shared.GridView");
        }
    }
}
