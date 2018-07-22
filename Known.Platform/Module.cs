using System.Collections.Generic;

namespace Known.Platform
{
    public enum ModuleViewType
    {
        DataGridView,
        TreeGridView,
        TabPageView,
        SplitPageView
    }

    public class Module
    {
        public string Id { get; set; }
        public Module Parent { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public ModuleViewType ViewType { get; set; }
        public List<Module> Children { get; set; }
        public List<Button> Buttons { get; set; }
        public List<Field> Fields { get; set; }
    }
}
