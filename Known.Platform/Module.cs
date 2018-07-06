using System.Collections.Generic;

namespace Known.Platform
{
    public class Module
    {
        public string Id { get; set; }
        public Module Parent { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public List<Module> Children { get; set; }
        public List<Button> Buttons { get; set; }
    }
}
