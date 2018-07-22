using System.Collections.Generic;

namespace Known.Platform
{
    public class Button
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool IsSplit { get; set; }
        public List<Button> Children { get; set; }
    }
}