using System.Collections.Generic;

namespace Known.Platform
{
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Token { get; set; }
        public Company Company { get; set; }
        public Department Department { get; set; }
        public User Manager { get; set; }
        public List<User> Proxys { get; set; }
        public List<Application> Applications { get; set; }
    }
}
