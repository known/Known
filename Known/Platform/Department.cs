using System.Collections.Generic;

namespace Known.Platform
{
    public class Department
    {
        public string Id { get; set; }
        public Department Parent { get; set; }
        public string Name { get; set; }
        public User Manager { get; set; }
        public List<Department> Children { get; set; }
    }
}