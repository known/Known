using System;
using System.Collections.Generic;

namespace Known
{
    public class PageView
    {
        private readonly List<string> styleFiles;
        private readonly List<string> scriptFiles;

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

        public Type Module { get; }

        public int Order { get; }

        public string Code { get; }

        public string Name { get; }

        public string Icon { get; }

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
