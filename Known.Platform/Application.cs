using System.Collections.Generic;

namespace Known.Platform
{
    public class Application
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Module> Modules { get; set; }
    }
}
