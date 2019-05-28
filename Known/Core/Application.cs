using System.Collections.Generic;

namespace Known.Core
{
    public class Application
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public List<Module> Modules { get; set; }
    }
}