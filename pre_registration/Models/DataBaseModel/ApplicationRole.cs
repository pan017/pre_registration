using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models.DataBaseModel
{
    public class ApplicationRole
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public ApplicationRole()
        {
            Users = new List<ApplicationUser>();
        }
        public ApplicationRole(string roleName)
        {
            this.Name = roleName;
            Users = new List<ApplicationUser>();
        }
    }
}
