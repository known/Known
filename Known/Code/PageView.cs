using System;
using System.Collections.Generic;

namespace Known
{
    /// <summary>
    /// 模块页面视图类。
    /// </summary>
    public class PageView
    {
        private readonly List<string> styleFiles;
        private readonly List<string> scriptFiles;

        /// <summary>
        /// 初始化一个模块页面视图类的实例。
        /// </summary>
        /// <param name="module">模块类型。</param>
        /// <param name="order">页面序号。</param>
        /// <param name="code">页面代码。</param>
        /// <param name="name">页面名称。</param>
        /// <param name="icon">页面图标。</param>
        public PageView(Type module, int order, string code, string name, string icon)
        {
            Module = module;
            Order = order;
            Code = code;
            Name = name;
            Icon = icon;
            styleFiles = new List<string>();
            scriptFiles = new List<string>();
        }

        /// <summary>
        /// 取得模块类型。
        /// </summary>
        public Type Module { get; }

        /// <summary>
        /// 取得页面序号。
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// 取得页面代码。
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// 取得页面名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 取得页面图标。
        /// </summary>
        public string Icon { get; }

        /// <summary>
        /// 添加页面 Css 样式文件。
        /// </summary>
        /// <param name="fileName">样式文件。</param>
        public void AddStyle(string fileName)
        {
            if (styleFiles.Contains(fileName))
                return;

            styleFiles.Add(fileName);
        }

        /// <summary>
        /// 添加页面 Javascript 脚本文件。
        /// </summary>
        /// <param name="fileName">脚本文件。</param>
        public void AddScript(string fileName)
        {
            if (scriptFiles.Contains(fileName))
                return;

            scriptFiles.Add(fileName);
        }

        /// <summary>
        /// 初始化页面。
        /// </summary>
        public void Init()
        {

        }
    }
}
