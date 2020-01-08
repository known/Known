using System;
using System.Collections.Generic;

namespace Known
{
    public class PageView
    {
        private readonly List<string> styleFiles;
        private readonly List<string> scriptFiles;

        public PageView(Type moduleType, string viewCode, string viewName)
        {
            ModuleType = moduleType;
            ViewCode = viewCode;
            ViewName = viewName;
            styleFiles = new List<string>();
            scriptFiles = new List<string>();
        }

        public Type ModuleType { get; }

        public string ViewCode { get; }

        public string ViewName { get; }

        public void AddStyle(string fileName)
        {
            if (styleFiles.Contains(fileName))
                return;

            styleFiles.Add(fileName);
        }

        public void AddScript(string fileName)
        {
            if (scriptFiles.Contains(fileName))
                return;

            scriptFiles.Add(fileName);
        }

        public void Init()
        {

        }
    }
}
