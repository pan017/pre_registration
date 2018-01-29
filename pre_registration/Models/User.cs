using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pre_registration.Models
{
    public class User:IdentityUser
    {
        public string Name { get; set; }
        public virtual string Area { get; set; }
    }
}
