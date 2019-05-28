using System.Collections.Generic;

namespace Known.Core
{
    public class Company
    {
        public string Id { get; set; }
        public Company Parent { get; set; }
        public string Name { get; set; }
        public List<Company> Children { get; set; }
    }
}