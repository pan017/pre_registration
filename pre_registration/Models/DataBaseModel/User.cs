using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace pre_registration.Models
{
    public class User:IdentityUser<int>
    {
        public int? AreaId { get; set; }
        public Area Area { get; set; }
        
        public int UserDataID { get; set; }
        public UserData UserData { get; set; }
    }
}
