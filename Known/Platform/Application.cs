using System.Collections.Generic;

namespace Known.Platform
{
    public class Application
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public List<Module> Modules { get; set; }
    }
}